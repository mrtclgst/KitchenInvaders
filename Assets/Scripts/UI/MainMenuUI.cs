using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playSingleplayerButton;
    [SerializeField] private Button _playMultiplayerButton;
    [SerializeField] private Button _quitButton;

    private void Awake()
    {
        _playSingleplayerButton.onClick.AddListener(OnSingleplayerButtonClicked);
        _playMultiplayerButton.onClick.AddListener(OnMultiplayerButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
        Time.timeScale = 1f;
    }
    private void OnSingleplayerButtonClicked()
    {
        KitchenGameMultiplayer.PlayMultiplayer = false;
        Loader.LoadScene(Loader.Scene.LobbyScene);
    }
    private void OnMultiplayerButtonClicked()
    {
        KitchenGameMultiplayer.PlayMultiplayer = true;
        Loader.LoadScene(Loader.Scene.LobbyScene);
    }
    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

}