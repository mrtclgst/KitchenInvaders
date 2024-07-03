using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BakingRecipeSO", menuName = "ScriptableObjects/BakingRecipeSO", order = 1)]
public class BakingRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO _input;
    [SerializeField] private KitchenObjectSO _output;
    [SerializeField] private float _bakingTimeMax;

    public KitchenObjectSO GetInput()
    {
        return _input;
    }

    public KitchenObjectSO GetOutput()
    {
        return _output;
    }

    public float GetBakingTimeMax()
    {
        return _bakingTimeMax;
    }
}