using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.Event_OnLocalPlayerReadyChanged += GameManager_Event_OnLocalPlayerReadyChanged;
        GameManager.Instance.Event_OnGameStateChanged += GameManager_Event_OnGameStateChanged;
        Hide();
    }

    private void GameManager_Event_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.IsLocalPlayerReady())
        {
            Show();
        }
    }
    private void GameManager_Event_OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
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
