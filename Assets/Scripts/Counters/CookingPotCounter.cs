using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CookingPotCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.Event_OnProgressChangedArgs> Event_OnProgressChanged;
    public event EventHandler<Event_OnStateChangedArgs> Event_OnStateChanged;

    public class Event_OnStateChangedArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Boiling,
    }


    [SerializeField] private BoilingRecipeSO[] _boilingRecipeSOArray;


    //private float _boilingTimer;
    //private State _state = State.Idle;
    private NetworkVariable<float> _boilingTimer = new NetworkVariable<float>();
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private BoilingRecipeSO _boilingRecipeSO;

    private void Start()
    {
        _state.Value = State.Idle;
        _boilingTimer.Value = 0f;
    }
    public override void OnNetworkSpawn()
    {
        _boilingTimer.OnValueChanged += BoilingTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }
    private void Update()
    {
        if (!IsServer) { return; }

        if (HasKitchenObject())
        {
            switch (_state.Value)
            {
                case State.Idle:
                    break;
                case State.Boiling:
                    _boilingTimer.Value += Time.deltaTime;
                    if (_boilingTimer.Value >= _boilingRecipeSO.GetBoilingTimeMax())
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.CreateKitchenObject(_boilingRecipeSO.GetOutput(), this);
                        _state.Value = State.Idle;
                        _boilingTimer.Value = 0;
                    }
                    break;
                default:
                    Debug.LogError("Invalid state: " + _state.Value);
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
                    SetStateIdleServerRpc();
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
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
    private void State_OnValueChanged(State previousValue, State newValue)
    {
        Event_OnStateChanged?.Invoke(this, new Event_OnStateChangedArgs { state = _state.Value });
        if (_state.Value == State.Idle)
        {
            Event_OnProgressChanged?.Invoke
                (this, new IHasProgress.Event_OnProgressChangedArgs { ProgressNormalized = 0f });
        }
    }
    private void BoilingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float boilingTimerMax = _boilingRecipeSO != null ? _boilingRecipeSO.GetBoilingTimeMax() : 1f;

        Event_OnProgressChanged?.Invoke
            (this, new IHasProgress.Event_OnProgressChangedArgs { ProgressNormalized = _boilingTimer.Value / boilingTimerMax });
    }
    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        BoilingRecipeSO boilingRecipeSO = GetBoilingRecipeSO(inputKitchenObjectSO);
        return boilingRecipeSO != null;
    }
    private BoilingRecipeSO GetBoilingRecipeSO(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BoilingRecipeSO boilingRecipeSO in _boilingRecipeSOArray)
        {
            if (boilingRecipeSO.GetInput() == inputKitchenObjectSO)
            {
                return boilingRecipeSO;
            }
        }
        return null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        _state.Value = State.Idle;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        _boilingTimer.Value = 0f;
        _state.Value = State.Boiling;
        SetBoilingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetBoilingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _boilingRecipeSO = GetBoilingRecipeSO(kitchenObjectSO);
    }
}