using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _recipeTemplate;

    private void Awake()
    {
        _recipeTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        DeliveryManager.Instance.Event_OnRecipeSpawned += OnRecipeSpawned;
        DeliveryManager.Instance.Event_OnRecipeDelivered += OnRecipeDelivered;
        UpdateVisuals();
    }

    private void OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisuals();
    }

    private void OnRecipeDelivered(object sender, EventArgs e)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (Transform child in _container)
        {
            if (child == _recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipe in DeliveryManager.Instance.GetWaitingSOList())
        {
            Transform recipeTemplateTransform = Instantiate(_recipeTemplate, _container);
            recipeTemplateTransform.gameObject.SetActive(true);
            DeliveryManagerSingleUI recipeUI = recipeTemplateTransform.GetComponent<DeliveryManagerSingleUI>();
            recipeUI.SetRecipeUI(recipe);
        }
    }
}