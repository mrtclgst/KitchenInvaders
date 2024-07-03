using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.Event_OnProgressChangedArgs> Event_OnProgressChanged;

    [SerializeField] private BakingRecipeSO[] _bakingRecipeSOArray;
    private float _bakingTimer;
    private BakingRecipeSO _bakingRecipeSO;
    private State _state = State.Idle;

    private enum State
    {
        Idle,
        Baking,
        Baked
    }

    private void Start()
    {
        _bakingTimer = 0f;
        _state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (_state)
            {
                case State.Idle:
                    break;
                case State.Baking:
                    _bakingTimer += Time.deltaTime;
                    if (_bakingTimer >= _bakingRecipeSO.GetBakingTimeMax())
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.CreateKitchenObject(_bakingRecipeSO.GetOutput(), this);
                        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
                        { ProgressNormalized = _bakingTimer / _bakingRecipeSO.GetBakingTimeMax() });
                        _bakingTimer = 0f;
                        _state = State.Baked;
                    }
                    break;
                case State.Baked:
                    break;
                default:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                //burada player'in tabagina eger urunumuz pistiyse urunu verecegiz.
                if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO());
                }
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                //burada playerin elindeki pot'u yakalamaya calisacagiz ve pottaki itemi alip buraya eklemeye calisacagiz.
                //if(player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))

            }
        }
    }
    private bool HasOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (BakingRecipeSO bakingRecipeSO in _bakingRecipeSOArray)
        {
            if (bakingRecipeSO.GetInput() == kitchenObjectSO)
            {
                _bakingRecipeSO = bakingRecipeSO;
                return true;
            }
        }
        return false;
    }

    public bool IsBaking()
    {
        return _state == State.Baking;
    }
}