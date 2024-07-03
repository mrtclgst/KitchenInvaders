using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image _clockImage;

    private void Start()
    {
        GameManager.Instance.Event_OnGameStateChanged += OnGameStateChanged;
        Hide();
    }
    private void Update()
    {
        float timerNormalized = GameManager.Instance.GetPlayingTimerNormalized();
        _clockImage.fillAmount = timerNormalized;
    }
    private void OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
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
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}