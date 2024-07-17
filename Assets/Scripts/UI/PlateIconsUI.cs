using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private Transform _iconTemplate;

    private void Awake()
    {
        _iconTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        _plateKitchenObject.Event_OnIngredientAdded += OnIngredientAdded;
        _plateKitchenObject.Event_OnIngredientRemoved += OnIngredientRemoved;
    }

    

    private void OnIngredientAdded(object sender, PlateKitchenObject.EventArgs_OnIngredientAdded e)
    {
        UpdateIcons();
    }
    private void OnIngredientRemoved(object sender, EventArgs e)
    {
        UpdateIcons();
    }
    private void UpdateIcons()
    {
        foreach (Transform child in transform)
        {
            if (child == _iconTemplate)
                continue;

            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in _plateKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconUI = Instantiate(_iconTemplate, transform);
            iconUI.gameObject.SetActive(true);
            iconUI.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}