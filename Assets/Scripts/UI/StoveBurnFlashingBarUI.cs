using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string ANIMATOR_BOOL_FLASH = "IsFlashing";
    [SerializeField] private StoveCounter _stoveCounter;
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _stoveCounter.Event_OnProgressChanged += OnProgressChanged;
        _animator.SetBool(ANIMATOR_BOOL_FLASH, false);
    }

    private void OnProgressChanged(object sender, IHasProgress.Event_OnProgressChangedArgs e)
    {
        float progressValueWillBeWarned = 0.5f;
        bool isFlashing = _stoveCounter.IsFrying() && e.ProgressNormalized > progressValueWillBeWarned;
        _animator.SetBool(ANIMATOR_BOOL_FLASH, isFlashing);
    }
}