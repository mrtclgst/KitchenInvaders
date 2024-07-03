using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObjectListSO", menuName = "ScriptableObjects/KitchenObjectListSO", order = 1)]
public class KitchenObjectListSO : ScriptableObject
{
    public List<KitchenObjectSO> KitchenObjectSOList;
}
