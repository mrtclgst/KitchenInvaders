using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _lobbyNameText;

    private Lobby _lobby;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnLobbyButtonClicked);
    }

    private void OnLobbyButtonClicked()
    {
        KitchenGameLobby.Instance.JoinLobbyWithId(_lobby.Id);
    }

    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyNameText.text = _lobby.Name;
    }
}
