using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button _createGameButton;
    [SerializeField] private Button _joinGameButton;

    private void Awake()
    {
        _createGameButton.onClick.AddListener(OnCreateGameButtonClicked);
        _joinGameButton.onClick.AddListener(OnJoinGameButtonClicked);
    }
    private void OnCreateGameButtonClicked()
    {
        KitchenGameMultiplayer.Instance.StartHost();
        Loader.LoadNetworkScene(Loader.Scene.CharacterSelectScene);
    }
    private void OnJoinGameButtonClicked()
    {
        KitchenGameMultiplayer.Instance.StartClient();
    }
}