using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    [SerializeField] private ContainerCounter _containerCounter;
    private Animator _animator;
    private const string OPEN_CLOSE = "OpenClose";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _containerCounter.Event_OnPlayerGrabbedObject += OnPlayerGrabbedObject;
    }

    private void OnPlayerGrabbedObject(object sender, EventArgs e)
    {
        _animator.SetTrigger(OPEN_CLOSE);
    }
}