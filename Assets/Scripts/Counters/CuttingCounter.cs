using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler Event_OnAnyCut;
    public event EventHandler Event_OnCut;
    public event EventHandler<IHasProgress.Event_OnProgressChangedArgs> Event_OnProgressChanged;

    new public static void ResetStaticData()
    {
        Event_OnAnyCut = null;
    }

    [SerializeField] private CuttingRecipeSO[] _cutKitchenObjectSO;
    private int _cuttingProgress;

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //player.GetKitchenObject().SetKitchenObjectParent(this);
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
            else
            {
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        _cuttingProgress = 0;
        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        { ProgressNormalized = 0f });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasOutputForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    public bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }
    public KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(input);

        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.GetOutput();
        }
        else
        {
            return null;
        }
    }
    private CuttingRecipeSO GetCuttingRecipeSO(KitchenObjectSO input)
    {
        foreach (CuttingRecipeSO cuttingRecipe in _cutKitchenObjectSO)
        {
            if (cuttingRecipe.GetInput() == input)
            {
                return cuttingRecipe;
            }
        }

        return null;
    }
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasKitchenObject() && HasOutputForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectClientRpc();
        }
    }
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        _cuttingProgress++;
        Event_OnCut?.Invoke(this, EventArgs.Empty);
        Event_OnAnyCut?.Invoke(this, EventArgs.Empty);
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO());
        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        { ProgressNormalized = (float)_cuttingProgress / cuttingRecipeSO.GetCuttingProgressMax() });
        TestCuttingProgressDoneServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if (HasKitchenObject() && HasOutputForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO());
            if (_cuttingProgress >= cuttingRecipeSO.GetCuttingProgressMax())
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
                //GetKitchenObject().DestroySelf();
                KitchenObject.CreateKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }
}