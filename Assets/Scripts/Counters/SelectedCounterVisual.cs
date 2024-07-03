using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter _baseCounter;
    [SerializeField] private GameObject[] _selectedVisual;

    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.Event_OnSelectedCounterChanged += OnSelectedCounterChanged;
            Debug.Log(Player.LocalInstance);
        }
        else
        {
            Player.Event_OnAnyPlayerSpawned += OnAnyPlayerSpawned;
        }
    }
    private void OnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.Event_OnSelectedCounterChanged -= OnSelectedCounterChanged;
            Player.LocalInstance.Event_OnSelectedCounterChanged += OnSelectedCounterChanged;
        }
    }
    private void OnSelectedCounterChanged(object sender, Player.Event_OnSelectedCounterChangedEventArgs e)
    {
        if (e.SelectedCounter == _baseCounter)
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
        foreach (GameObject visualGameObject in _selectedVisual)
        {
            visualGameObject.SetActive(true);
        }
    }
    private void Hide()
    {
        foreach (GameObject visualGameObject in _selectedVisual)
        {
            visualGameObject.SetActive(false);
        }
    }

}