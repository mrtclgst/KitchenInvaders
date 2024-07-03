using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _resultText;

    [Header("Data")]
    [SerializeField] private Color _successColor;
    [SerializeField] private Color _failedColor;
    [SerializeField] private Sprite _successSprite;
    [SerializeField] private Sprite _failedSprite;

    private const string ANIMATOR_TRIGGER_POPUP = "Popup";

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.Event_OnRecipeSuccess += OnRecipeSuccess;
        DeliveryManager.Instance.Event_OnRecipeFailed += OnRecipeFailed;
        gameObject.SetActive(false);
    }

    private void OnRecipeSuccess(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(ANIMATOR_TRIGGER_POPUP);
        _backgroundImage.color = _successColor;
        _iconImage.sprite = _successSprite;
        _resultText.text = "DELIVERY\nSUCCESS";
    }
    private void OnRecipeFailed(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(ANIMATOR_TRIGGER_POPUP);
        _backgroundImage.color = _failedColor;
        _iconImage.sprite = _failedSprite;
        _resultText.text = "DELIVERY\nFAILED";
    }
}
