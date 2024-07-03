using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverPlate(plateKitchenObject);
                KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                //player.GetKitchenObject().DestroySelf();
            }
        }
    }
}