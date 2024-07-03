using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/BurningRecipeSO", order = 1)]
public class BurningRecipeSO : ScriptableObject
{
    [SerializeField] private KitchenObjectSO _input;
    [SerializeField] private KitchenObjectSO _output;
    [SerializeField] private float _burningTimeMax;

    public KitchenObjectSO GetInput()
    {
        return _input;
    }
    public KitchenObjectSO GetOutput()
    {
        return _output;
    }
    public float GetBurningTimeMax()
    {
        return _burningTimeMax;
    }
}