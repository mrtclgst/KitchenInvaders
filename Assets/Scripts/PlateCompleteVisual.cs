using System;
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
    [Serializable]
    public struct RecipeVisual
    {
        public List<KitchenObjectSO> requiredIngredients;
        public GameObject recipeGameObject;
    }

    [SerializeField] private PlateKitchenObject _plateKitchenObject;
    [SerializeField] private List<IngredientVisual> _ingredientVisualList;
    [SerializeField] private List<RecipeVisual> _recipeVisualList;


    private void Start()
    {
        _plateKitchenObject.Event_OnIngredientAdded += OnIngredientAdded;
        _plateKitchenObject.Event_OnIngredientRemoved += OnIngredientRemoved;
        foreach (IngredientVisual ingredientVisual in _ingredientVisualList)
        {
            ingredientVisual.gameObject.SetActive(false);
        }
        foreach (RecipeVisual recipeVisual in _recipeVisualList)
        {
            recipeVisual.recipeGameObject.SetActive(false);
        }
    }

    private void OnIngredientRemoved(object sender, EventArgs e)
    {
        foreach (IngredientVisual ingredientVisual in _ingredientVisualList)
        {
            ingredientVisual.gameObject.SetActive(false);
        }
        foreach (RecipeVisual recipeVisual in _recipeVisualList)
        {
            recipeVisual.recipeGameObject.SetActive(false);
        }
    }

    private void OnIngredientAdded(object sender, PlateKitchenObject.EventArgs_OnIngredientAdded e)
    {
        List<KitchenObjectSO> currentIngredients = _plateKitchenObject.GetKitchenObjectSOList();

        if (currentIngredients.Count > 1)
        {
            foreach (RecipeVisual recipeVisual in _recipeVisualList)
            {
                if (IsRecipeMatch(recipeVisual.requiredIngredients, currentIngredients))
                {
                    foreach (IngredientVisual ingredientVisual in _ingredientVisualList)
                    {
                        ingredientVisual.gameObject.SetActive(false);
                    }
                    foreach(RecipeVisual recipeVisualChecker in _recipeVisualList)
                    {
                        recipeVisualChecker.recipeGameObject.SetActive(false);
                    }
                    recipeVisual.recipeGameObject.SetActive(true);
                    return;
                }
                else
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
        }
        else
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
    private bool IsRecipeMatch(List<KitchenObjectSO> recipeIngredients, List<KitchenObjectSO> currentIngredients)
    {
        if (recipeIngredients.Count != currentIngredients.Count)
        {
            return false;
        }

        foreach (KitchenObjectSO ingredient in recipeIngredients)
        {
            if(!currentIngredients.Contains(ingredient))
            {
                return false;
            }
        }

        return true;
    }
}