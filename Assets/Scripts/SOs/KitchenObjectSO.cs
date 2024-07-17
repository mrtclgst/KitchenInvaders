using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenInvaders", menuName = "ScriptableObjects/KitchenObjectSO", order = 0)]
public class KitchenObjectSO : ScriptableObject
{
    [SerializeField] private Transform _prefab;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;

    public Transform GetPrefab()
    {
        return _prefab;
    }
    public Sprite GetIcon()
    {
        return _icon;
    }
    public string GetName()
    {
        return _name;
    }
}