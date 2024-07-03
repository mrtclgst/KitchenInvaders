using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button _soundEffectsButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _soundEffectText;
    [SerializeField] private TextMeshProUGUI _musicText;
    [SerializeField] private Transform _pressToRebindKeyTransform;

    [SerializeField] private Button _moveUpButton;
    [SerializeField] private Button _moveDownButton;
    [SerializeField] private Button _moveLeftButton;
    [SerializeField] private Button _moveRightButton;
    [SerializeField] private Button _interactButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _altInteractButton;
    [SerializeField] private Button _gamepadInteractButton;
    [SerializeField] private Button _gamepadPauseButton;
    [SerializeField] private Button _gamepadAltInteractButton;

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

    private Action onClosedButtonAction;

    private void Awake()
    {
        Instance = this;
        _soundEffectsButton.onClick.AddListener(OnSoundEffectsButtonClick);
        _musicButton.onClick.AddListener(OnMusicButtonClick);
        _backButton.onClick.AddListener(OnBackButtonClick);

        _moveUpButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Up));
        _moveDownButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Down));
        _moveLeftButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Left));
        _moveRightButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Right));
        _interactButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
        _pauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));
        _altInteractButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlternate));
        _gamepadInteractButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Interact));
        _gamepadPauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Pause));
        _gamepadAltInteractButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_InteractAlternate));
    }
    private void Start()
    {
        GameManager.Instance.Event_OnLocalGameUnpaused += OnGameUnpaused;
        UpdateVisuals();
        HidePressToRebindKey();
        Hide();
    }
    private void OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }
    private void OnSoundEffectsButtonClick()
    {
        SoundManager.Instance.ChangeVolume();
        UpdateVisuals();
    }
    private void OnMusicButtonClick()
    {
        MusicManager.Instance.ChangeVolume();
        UpdateVisuals();
    }
    private void OnBackButtonClick()
    {
        Hide();
        onClosedButtonAction?.Invoke();
    }
    private void UpdateVisuals()
    {
        _soundEffectText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10);
        _musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10);
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
    public void Show(Action onCloseButtonAction)
    {
        this.onClosedButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);
        _soundEffectsButton.Select();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private void ShowPressToRebindKey()
    {
        _pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    private void HidePressToRebindKey()
    {
        _pressToRebindKeyTransform.gameObject.SetActive(false);
    }
    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdateVisuals();
        });
    }
}
