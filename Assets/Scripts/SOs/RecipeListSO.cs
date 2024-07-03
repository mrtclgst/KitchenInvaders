using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/RecipeSOList", order = 1)]
public class RecipeListSO : ScriptableObject
{
    [SerializeField] private List<RecipeSO> _recipeSOList;

    public RecipeSO GetRandomRecipe()
    {
        return _recipeSOList[UnityEngine.Random.Range(0, _recipeSOList.Count)];
    }
    public List<RecipeSO> GetRecipeSOList()
    {
        return _recipeSOList;
    }
    
    internal RecipeSO GetRecipeAtIndex(int recipeSOIndex)
    {
        return _recipeSOList[recipeSOIndex];
    }
}
