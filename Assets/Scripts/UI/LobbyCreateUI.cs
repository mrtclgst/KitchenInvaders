using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyNameInputField;
    [SerializeField] private Button _createPublicLobbyButton;
    [SerializeField] private Button _createPrivateLobbyButton;
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        _createPublicLobbyButton.onClick.AddListener(OnCreatePublicLobbyButtonClicked);
        _createPrivateLobbyButton.onClick.AddListener(OnCreatePrivateLobbyButtonClicked);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    private void Start()
    {
        Hide();
    }
    private void OnCreatePublicLobbyButtonClicked()
    {
        KitchenGameLobby.Instance.CreateLobby(_lobbyNameInputField.text, false);
    }
    private void OnCreatePrivateLobbyButtonClicked()
    {
        KitchenGameLobby.Instance.CreateLobby(_lobbyNameInputField.text, true);
    }
    private void OnCloseButtonClicked()
    {
        Hide();
    }
    internal void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}