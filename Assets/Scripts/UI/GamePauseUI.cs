using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _optionsButton;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _optionsButton.onClick.AddListener(OnOptionsButtonClicked);
    }

    private void Start()
    {
        GameManager.Instance.Event_OnLocalGamePaused += OnLocalGamePaused;
        GameManager.Instance.Event_OnLocalGameUnpaused += OnLocalGameUnpaused;
        Hide();
    }

    private void OnResumeButtonClicked()
    {
        GameManager.Instance.TogglePauseGame();
    }

    private void OnMainMenuButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.LoadScene(Loader.Scene.MainMenuScene);
    }
    private void OnOptionsButtonClicked()
    {
        Hide();
        OptionsUI.Instance.Show(Show);
    }

    private void OnLocalGamePaused(object sender, EventArgs e)
    {
        Show();
        _resumeButton.Select();
    }

    private void OnLocalGameUnpaused(object sender, EventArgs e)
    {
        Hide();
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
