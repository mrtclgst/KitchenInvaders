using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectParent
{
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;

    public override void Interact(Player player)
    {
        //counter has kitchen object
        if (HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                //player has plate
                if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //GetKitchenObject().DestroySelf();
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    if (GetKitchenObject().TryToGetPlate(out PlateKitchenObject counterPlate))
                    {
                        if (counterPlate.TryToAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                            //player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        //counter does not have kitchen object
        else
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);

            }
            else
            {

            }
        }
    }
}
