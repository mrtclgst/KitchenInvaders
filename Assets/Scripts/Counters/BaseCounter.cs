using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler Event_OnAnyObjectPlacedHere;
    [SerializeField] private Transform _counterTopPoint;

    private KitchenObject _kitchenObject;

    public static void ResetStaticData()
    {
        Event_OnAnyObjectPlacedHere = null;
    }

    public virtual void Interact(Player player)
    {
        Debug.Log("Interacting with base counter");
    }
    public virtual void InteractAlternate(Player player)
    {
        Debug.Log("AlternateInteraction with base counter");
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            Event_OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }
    public Transform GetCounterTopPoint()
    {
        return _counterTopPoint;
    }
    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }
    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return _counterTopPoint;
    }
}