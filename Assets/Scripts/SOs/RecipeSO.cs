using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/RecipeSO", order = 1)]
public class RecipeSO : ScriptableObject
{
    [SerializeField] private List<KitchenObjectSO> _kitchenObjectSOList;
    [SerializeField] string _recipeName;

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return _kitchenObjectSOList;
    }
    public string GetRecipeName()
    {
        return _recipeName;
    }
}