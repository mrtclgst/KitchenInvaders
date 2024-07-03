using System;
using Unity.Netcode;
using UnityEngine;

public class MixerCounter : BaseCounter, IHasProgress
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
        Mixing,
    }

    [SerializeField] private MixingRecipeSO[] _mixingRecipeSOArray;

    private NetworkVariable<float> _mixerTimer = new NetworkVariable<float>();
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private MixingRecipeSO _mixingRecipeSO;

    private void Update()
    {
        if (!IsServer) { return; }
        if (HasKitchenObject())
        {
            switch (_state.Value)
            {
                case State.Idle:
                    break;
                case State.Mixing:
                    _mixerTimer.Value += Time.deltaTime;
                    if (_mixerTimer.Value > _mixingRecipeSO.GetFryingTimeMax())
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.CreateKitchenObject(_mixingRecipeSO.GetOutput(), this);
                        _state.Value = State.Idle;
                        _mixerTimer.Value = 0;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public override void OnNetworkSpawn()
    {
        _mixerTimer.OnValueChanged += MixingTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }
    private void MixingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float mixingTimerMax = _mixingRecipeSO != null ? _mixingRecipeSO.GetFryingTimeMax() : 1f;
        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        { ProgressNormalized = _mixerTimer.Value / mixingTimerMax });
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
    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        MixingRecipeSO mixingRecipeSO = GetMixingRecipeSO(inputKitchenObjectSO);
        return mixingRecipeSO != null;
    }
    private MixingRecipeSO GetMixingRecipeSO(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (MixingRecipeSO mixingRecipeSO in _mixingRecipeSOArray)
        {
            if (mixingRecipeSO.GetInput() == inputKitchenObjectSO)
            {
                return mixingRecipeSO;
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
        _mixerTimer.Value = 0;
        _state.Value = State.Mixing;
        SetMixingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetMixingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _mixingRecipeSO = GetMixingRecipeSO(kitchenObjectSO);
    }
}