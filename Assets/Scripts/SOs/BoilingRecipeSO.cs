using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/BoilingRecipeSO", order = 1)]
public class BoilingRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO _input;
    [SerializeField] private KitchenObjectSO _output;
    [SerializeField] private float _boilingTimeMax;

    public KitchenObjectSO GetInput()
    {
        return _input;
    }
    public KitchenObjectSO GetOutput()
    {
        return _output;
    }
    public float GetBoilingTimeMax()
    {
        return _boilingTimeMax;
    }
}