using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<EventArgs_OnIngredientAdded> Event_OnIngredientAdded;
    public event EventHandler Event_OnIngredientRemoved;
    public class EventArgs_OnIngredientAdded : EventArgs
    {
        public KitchenObjectSO ingredientSO;
    }

    [SerializeField] private List<KitchenObjectSO> _validKitchenObjectSOList;
    private List<KitchenObjectSO> _kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();
        _kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryToAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!_validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        if (_kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        else
        {
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
    }

    public void RemoveIngredientsFromPlate()
    {
        RemoveIngredientServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _kitchenObjectSOList.Add(kitchenObjectSO);
        Event_OnIngredientAdded?.Invoke(this, new EventArgs_OnIngredientAdded { ingredientSO = kitchenObjectSO });
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveIngredientServerRPC()
    {
        _kitchenObjectSOList.Clear();
        Event_OnIngredientRemoved?.Invoke(this, EventArgs.Empty);
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return _kitchenObjectSOList;
    }
}

