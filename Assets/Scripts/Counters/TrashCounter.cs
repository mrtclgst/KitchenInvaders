using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler Event_OnAnyObjectTrashed;

    new public static void ResetStaticData()
    {
        Event_OnAnyObjectTrashed = null;
    }
    
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            //player.GetKitchenObject().DestroySelf();
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        Event_OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}