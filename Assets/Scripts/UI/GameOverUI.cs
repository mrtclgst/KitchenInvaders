using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _recipesDeliveredText;
    [SerializeField] private Button _mainMenuButton;

    private void Start()
    {
        GameManager.Instance.Event_OnGameStateChanged += OnGameStateChanged;
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        Hide();
    }

    private void OnMainMenuButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.LoadScene(Loader.Scene.MainMenuScene);
    }

    private void OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        _recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfullyDeliveredCount().ToString();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}