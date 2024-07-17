using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler Event_OnGameStateChanged;
    public event EventHandler Event_OnLocalGamePaused;
    public event EventHandler Event_OnLocalGameUnpaused;
    public event EventHandler Event_OnMultiplayerGamePaused;
    public event EventHandler Event_OnMultiplayerGameUnpaused;
    public event EventHandler Event_OnLocalPlayerReadyChanged;

    private enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlaying,
        GameOver
    }

    [SerializeField] private Transform _playerPrefab;
    [SerializeField] private float _gamePlayingTimerMax = 60;
    private NetworkVariable<State> _gameState = new NetworkVariable<State>(State.WaitingToStart);
    private bool _isLocalPlayerReady = false;
    private NetworkVariable<float> _countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> _gamePlayingTimer = new NetworkVariable<float>(0f);
    private bool _isLocalGamePaused = false;
    private bool _autoTestGamePausedState = false;
    private NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> _playerReadyDictionary;
    private Dictionary<ulong, bool> _playerPausedDictionary;

    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
        _playerPausedDictionary = new Dictionary<ulong, bool>();
    }
    private void Start()
    {
        GameInput.Instance.Event_OnPausePerformed += OnPausePerformed;
        GameInput.Instance.Event_OnInteractPerformed += OnInteractPerformed;
    }
    public override void OnNetworkSpawn()
    {
        _gameState.OnValueChanged += GameState_OnValueChanged;
        _isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

            //this is triggered when a client have loaded the scene
            //NetworkManager.Singleton.SceneManager.OnLoadComplete

            //this is triggered when all the clients have loaded the scene
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(_playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
        }
    }

    private void GameState_OnValueChanged(State previousValue, State newValue)
    {
        Event_OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        _autoTestGamePausedState = true;
        //TestGamePausedState();
    }
    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (_isGamePaused.Value)
        {
            Event_OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 0;
        }
        else
        {
            Event_OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
            Time.timeScale = 1;
        }
    }
    private void Update()
    {
        if (!IsServer)
            return;

        switch (_gameState.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountDownToStart:
                CountDownToStart();
                break;
            case State.GamePlaying:
                GamePlaying();

                break;
            case State.GameOver:
                GameOver();
                break;
        }
    }
    private void LateUpdate()
    {
        if (_autoTestGamePausedState)
        {
            _autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }
    private void CountDownToStart()
    {
        _countdownToStartTimer.Value -= Time.deltaTime;

        if (_countdownToStartTimer.Value <= 0f)
        {
            _gamePlayingTimer.Value = _gamePlayingTimerMax;
            ChangeState(State.GamePlaying);
        }
    }
    private void GamePlaying()
    {
        _gamePlayingTimer.Value -= Time.deltaTime;
        if (_gamePlayingTimer.Value <= 0f)
        {
            ChangeState(State.GameOver);
        }
    }
    private void GameOver()
    {
    }
    private void OnInteractPerformed(object sender, EventArgs e)
    {
        if (_gameState.Value is State.WaitingToStart)
        {
            _isLocalPlayerReady = true;
            Event_OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetReadyPlayerServerRpc();
            //ChangeState(State.CountDownToStart);
        }
    }
    private void OnPausePerformed(object sender, EventArgs e)
    {
        TogglePauseGame();
    }
    private void ChangeState(State state)
    {
        _gameState.Value = state;
        //Event_OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }
    public bool IsWaitingToStart()
    {
        return _gameState.Value is State.WaitingToStart;
    }
    public bool IsGamePlaying()
    {
        return _gameState.Value is State.GamePlaying;
    }
    public float GetCountdownTimer()
    {
        return _countdownToStartTimer.Value;
    }
    public bool IsCountdownToStartActive()
    {
        return _gameState.Value is State.CountDownToStart;
    }
    internal bool IsGameOver()
    {
        return _gameState.Value is State.GameOver;
    }
    internal bool IsLocalPlayerReady()
    {
        return _isLocalPlayerReady;
    }
    internal float GetPlayingTimerNormalized()
    {
        return 1 - (_gamePlayingTimer.Value / _gamePlayingTimerMax);
    }
    internal void TogglePauseGame()
    {
        _isLocalGamePaused = !_isLocalGamePaused;
        if (_isLocalGamePaused)
        {
            PauseGameServerRpc();
            Event_OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();
            Event_OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allPlayersReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientID) || !_playerReadyDictionary[clientID])
            {
                //this player is not ready
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            _gameState.Value = State.CountDownToStart;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }
    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }
    private void TestGamePausedState()
    {
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (_playerPausedDictionary.ContainsKey(clientID) && _playerPausedDictionary[clientID])
            {
                _isGamePaused.Value = true;
                return;
            }
        }

        _isGamePaused.Value = false;
    }
}