using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _quickJoinButton;
    [SerializeField] private Button _joinWithCodeButton;
    [SerializeField] private LobbyCreateUI _createLobbyUI;
    [SerializeField] private TMP_InputField _lobbyCodeInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;
    [SerializeField] private Transform _lobbyContainer;
    [SerializeField] private Transform _lobbyTemplate;


    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        _quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);
        _joinWithCodeButton.onClick.AddListener(OnJoinWithCodeButtonClicked);
    }


    private void Start()
    {
        _playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        _playerNameInputField.onValueChanged.AddListener(OnPlayerNameChanged);
        UpdateLobbyList(new List<Lobby>());
        _lobbyTemplate.gameObject.SetActive(false);
        KitchenGameLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
    }
    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
    }
    private void OnPlayerNameChanged(string playerNameText)
    {
        KitchenGameMultiplayer.Instance.SetPlayerName(playerNameText);
    }
    private void OnMainMenuButtonClicked()
    {
        KitchenGameLobby.Instance.LeaveLobby();
        Loader.LoadScene(Loader.Scene.MainMenuScene);
    }
    private void OnCreateLobbyButtonClicked()
    {
        _createLobbyUI.Show();
    }
    private void OnQuickJoinButtonClicked()
    {
        KitchenGameLobby.Instance.QuickJoin();
    }
    private void OnJoinWithCodeButtonClicked()
    {
        KitchenGameLobby.Instance.JoinLobbyWithCode(_lobbyCodeInputField.text);
    }
    private void KitchenGameLobby_OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.LobbyList);
    }
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyContainer)
        {
            if (child == _lobbyTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyUITransform = Instantiate(_lobbyTemplate, _lobbyContainer);
            lobbyUITransform.gameObject.SetActive(true);
            lobbyUITransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}