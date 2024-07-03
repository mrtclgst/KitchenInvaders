using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI _countdownText;

    private Animator _animator;
    private float _previousCountdownNumber;

    private void Awake()
    {
        _animator = GetComponent<Animator>();    
    }
    private void Start()
    {
        GameManager.Instance.Event_OnGameStateChanged += OnGameStateChanged;
        Hide();
    }
    private void Update()
    {
        int currentCountdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownTimer());
        _countdownText.text = currentCountdownNumber.ToString();

        if (currentCountdownNumber != _previousCountdownNumber)
        {
            _animator.SetTrigger(NUMBER_POPUP);
            _previousCountdownNumber = currentCountdownNumber;
            SoundManager.Instance.PlayCountdownSound();
        }
    }
    private void OnGameStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
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