using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    [SerializeField] private GameObject _readyText;
    [SerializeField] private PlayerVisual _playerVisual;
    [SerializeField] private Button _kickButton;
    [SerializeField] private TextMeshPro _playerNameText;

    private void Awake()
    {
        _kickButton.onClick.AddListener(KickButton_OnClick);
    }
    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyStatusChanged += CharacterSelectReady_OnReadyStatusChanged;
        _kickButton.gameObject.SetActive(KitchenGameMultiplayer.Instance.IsServer);
        UpdatePlayer();
    }
    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyStatusChanged -= CharacterSelectReady_OnReadyStatusChanged;
    }

    private void CharacterSelectReady_OnReadyStatusChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    private void KickButton_OnClick()
    {
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
        ulong clientId = playerData.clientId;
        KitchenGameLobby.Instance.KickPlayerAsync(playerData.playerId.ToString());
        KitchenGameMultiplayer.Instance.KickPlayer(clientId);
    }
    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            _readyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            _playerVisual.SetPlayerVisual(playerData.characterVisualId);
            _playerNameText.text = playerData.playerName.ToString();
        }
        else
        {
            Hide();
        }
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
