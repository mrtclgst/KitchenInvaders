using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct IngredientVisual
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }


    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private List<IngredientVisual> _ingredientVisualList;

    
    private void Start()
    {
        _plateKitchenObject.Event_OnIngredientAdded += OnIngredientAdded;
        foreach (IngredientVisual ingredientVisual in _ingredientVisualList)
        {
            ingredientVisual.gameObject.SetActive(false);
        }
    }

    private void OnIngredientAdded(object sender, PlateKitchenObject.EventArgs_OnIngredientAdded e)
    {
        foreach (IngredientVisual ingredientVisual in _ingredientVisualList)
        {
            if (e.ingredientSO == ingredientVisual.kitchenObjectSO)
            {
                ingredientVisual.gameObject.SetActive(true);
            }
        }
    }
}