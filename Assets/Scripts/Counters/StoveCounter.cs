using System;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<Event_OnStateChangedArgs> Event_OnStateChanged;
    public event EventHandler<IHasProgress.Event_OnProgressChangedArgs> Event_OnProgressChanged;

    public class Event_OnStateChangedArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Burning,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] _fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] _burningRecipeSOArray;

    private NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0);
    private NetworkVariable<float> _burningTimer = new NetworkVariable<float>(0);
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private FryingRecipeSO _fryingRecipeSO;
    private BurningRecipeSO _burningRecipeSO;

    //private void Start()
    //{
    //    //_state = State.Idle;
    //    //_fryingTimer = 0f;
    //}

    public override void OnNetworkSpawn()
    {
        _fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        _burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }
    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = _fryingRecipeSO != null ? _fryingRecipeSO.GetFryingTimeMax() : 1f;

        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        { ProgressNormalized = _fryingTimer.Value / fryingTimerMax });
    }
    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = _burningRecipeSO != null ? _burningRecipeSO.GetBurningTimeMax() : 1f;
        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        { ProgressNormalized = _burningTimer.Value / burningTimerMax });
    }
    private void State_OnValueChanged(State previousValue, State newValue)
    {
        Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state.Value });

        if (_state.Value == State.Burned || _state.Value == State.Idle)
        {
            Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
            { ProgressNormalized = 0f });
        }
    }
    private void Update()
    {
        if (!IsServer)
            return;

        if (HasKitchenObject())
        {
            switch (_state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    _fryingTimer.Value += Time.deltaTime;
                    if (_fryingTimer.Value >= _fryingRecipeSO.GetFryingTimeMax())
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf();
                        KitchenObject.CreateKitchenObject(_fryingRecipeSO.GetOutput(), this);

                        _state.Value = State.Burning;
                        _burningTimer.Value = 0f;

                        //_burningRecipeSO = GetBurningRecipeSO(GetKitchenObject().GetKitchenObjectSO());
                        SetBurningRecipeSOClientRpc(
                            KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));

                        //Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state.Value });
                    }
                    break;
                case State.Burning:
                    _burningTimer.Value += Time.deltaTime;

                    //Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
                    //{ ProgressNormalized = _burningTimer.Value / _burningRecipeSO.GetBurningTimeMax() });

                    if (_burningTimer.Value > _burningRecipeSO.GetBurningTimeMax())
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        //GetKitchenObject().DestroySelf();
                        KitchenObject.CreateKitchenObject(_burningRecipeSO.GetOutput(), this);
                        _state.Value = State.Burned;

                        //Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state });
                        //Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
                        //{ ProgressNormalized = 0f });
                    }
                    break;
                case State.Burned:
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
                if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO());
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    //GetKitchenObject().DestroySelf();
                    SetStateIdleServerRpc();
                    //Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state });
                    //Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
                    //{ ProgressNormalized = 0f });
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
                //_state.Value = State.Idle;
                //Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state });
                //Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
                //{ ProgressNormalized = 0f });
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
                }
            }
            else
            {

            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        _state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        _fryingTimer.Value = 0f;
        _state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _fryingRecipeSO = GetFryingRecipeSO(kitchenObjectSO);
        //Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state });
        //Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        //{ ProgressNormalized = _fryingTimer.Value / _fryingRecipeSO.GetFryingTimeMax() });
    }
    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _burningRecipeSO = GetBurningRecipeSO(kitchenObjectSO);
        //Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state });
        //Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        //{ ProgressNormalized = _fryingTimer.Value / _fryingRecipeSO.GetFryingTimeMax() });
    }
    public bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSO(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }
    public KitchenObjectSO GetOutputForInput(KitchenObjectSO input)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSO(input);

        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.GetOutput();
        }
        else
        {
            return null;
        }
    }
    private FryingRecipeSO GetFryingRecipeSO(KitchenObjectSO input)
    {
        foreach (FryingRecipeSO fryingRecipe in _fryingRecipeSOArray)
        {
            if (fryingRecipe.GetInput() == input)
            {
                return fryingRecipe;
            }
        }

        return null;
    }
    private BurningRecipeSO GetBurningRecipeSO(KitchenObjectSO input)
    {
        foreach (BurningRecipeSO burningRecipe in _burningRecipeSOArray)
        {
            if (burningRecipe.GetInput() == input)
            {
                return burningRecipe;
            }
        }

        return null;
    }
    public bool IsFrying()
    {
        return _state.Value == State.Burning;
    }
}