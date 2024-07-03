using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/CuttingRecipeSO", order = 1)]
public class CuttingRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO _input;
    [SerializeField] private KitchenObjectSO _output;
    [SerializeField] private int _cuttingProgressMax;

    public KitchenObjectSO GetInput()
    {
        return _input;
    }
    public KitchenObjectSO GetOutput()
    {
        return _output;
    }
    public int GetCuttingProgressMax()
    {
        return _cuttingProgressMax;
    }
}