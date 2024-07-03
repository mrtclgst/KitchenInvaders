using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _startClientButton;
    private void Awake()
    {
        _startHostButton.onClick.AddListener(OnStartHostButtonClicked);
        _startClientButton.onClick.AddListener(OnStartClientButtonClicked);
    }

    private void OnStartHostButtonClicked()
    {
        Debug.Log("HOST");
        KitchenGameMultiplayer.Instance.StartHost();
        //NetworkManager.Singleton.StartHost();
        Hide();
    }
    private void OnStartClientButtonClicked()
    {
        Debug.Log("CLIENT");
        //NetworkManager.Singleton.StartClient();
        KitchenGameMultiplayer.Instance.StartClient();
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}