using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _moveUpText;
    [SerializeField] private TextMeshProUGUI _moveDownText;
    [SerializeField] private TextMeshProUGUI _moveLeftText;
    [SerializeField] private TextMeshProUGUI _moveRightText;
    [SerializeField] private TextMeshProUGUI _interactText;
    [SerializeField] private TextMeshProUGUI _pauseText;
    [SerializeField] private TextMeshProUGUI _altInteractText;
    [SerializeField] private TextMeshProUGUI _gamepadInteractText;
    [SerializeField] private TextMeshProUGUI _gamepadPauseText;
    [SerializeField] private TextMeshProUGUI _gamepadAltInteractText;

    private void Start()
    {
        GameInput.Instance.Event_OnBindingRebind += OnBindingRebind;
        //GameManager.Instance.Event_OnGameStateChanged += OnGameStateChanged;
        GameManager.Instance.Event_OnLocalPlayerReadyChanged += OnLocalPlayerReadyChanged;
        UpdateVisual();
        Show();
    }

    //private void OnGameStateChanged(object sender, EventArgs e)
    //{
    //    if (GameManager.Instance.IsCountdownToStartActive())
    //    {
    //        Hide();
    //    }
    //}
    private void OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }
    private void OnBindingRebind(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        _moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        _moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        _moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        _moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        _interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        _pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        _altInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        _gamepadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
        _gamepadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
        _gamepadAltInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
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