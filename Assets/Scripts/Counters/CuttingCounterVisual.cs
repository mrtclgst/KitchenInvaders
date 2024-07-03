using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private const string ANIMATOR_PARAM_CUT = "Cut";
    [SerializeField] private CuttingCounter _cuttingCounter;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _cuttingCounter.Event_OnCut += OnCut;
    }

    private void OnCut(object sender, EventArgs e)
    {
        _animator.SetTrigger(ANIMATOR_PARAM_CUT);
    }
}
