using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MixerCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.Event_OnProgressChangedArgs> Event_OnProgressChanged;
    public event EventHandler<Event_OnStateChangedArgs> Event_OnStateChanged;
    [Serializable]
    public struct MixerRecipe
    {
        public List<KitchenObjectSO> mixerRecipeIngredients;
        public float mixerTimerMax;
    }
    public class Event_OnStateChangedArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Mixing,
    }

    //[SerializeField] private MixingRecipeSO[] _mixingRecipeSOArray;
    [SerializeField] private List<MixerRecipe> _mixerRecipeList;
    [SerializeField] private List<KitchenObjectSO> _kitchenObjectSOList;
    [SerializeField] private KitchenObjectSO _mixedKitchenObjectSO;

    private NetworkVariable<float> _mixerTimer = new NetworkVariable<float>();
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> _mixerTimerMax = new NetworkVariable<float>();

    private void Update()
    {
        if (!IsServer) { return; }
        //if (HasKitchenObject())
        //{
        switch (_state.Value)
        {
            case State.Idle:
                break;
            case State.Mixing:
                _mixerTimer.Value += Time.deltaTime;
                if (_mixerTimer.Value > _mixerTimerMax.Value)
                {
                    //KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    //KitchenObject.CreateKitchenObject(_mixedKitchenObject, this);
                    SetMixingRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(_mixedKitchenObjectSO));
                    _state.Value = State.Idle;
                    _mixerTimer.Value = 0;
                }
                break;
            default:
                break;
        }
        //}
    }
    public override void OnNetworkSpawn()
    {
        _mixerTimer.OnValueChanged += MixingTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }
    private void MixingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float mixingTimerMax = _mixerTimerMax.Value != 0 ? _mixerTimerMax.Value : 1f;
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
        if (_kitchenObjectSOList.Count > 0)
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    foreach (KitchenObjectSO kitchenObjectSO in _kitchenObjectSOList)
                    {
                        plateKitchenObject.TryToAddIngredient(kitchenObjectSO);
                    }

                    //plateKitchenObject.TryToAddIngredient(GetKitchenObject().GetKitchenObjectSO());
                    //KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    SetStateIdleServerRpc();
                }
            }
            else
            {
                //GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryToGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    List<KitchenObjectSO> ingredientsInPlate = plateKitchenObject.GetKitchenObjectSOList();
                    if (HasOutputForInput(ingredientsInPlate))
                    {
                        foreach (KitchenObjectSO kitchenObjectSO in ingredientsInPlate)
                        {
                            InteractLogicPlaceObjectOnCounterServerRpc(
                                KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
                        }
                        plateKitchenObject.RemoveIngredientsFromPlate();
                    };
                }

                /*if (HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player'in tabagi alinacak ve tabaktaki malzemeleri kontrol edecegiz.
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlaceObjectOnCounterServerRpc(
                        KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
                }*/
            }
            else
            {

            }
        }
    }
    private bool HasOutputForInput(List<KitchenObjectSO> ingredientsInPlate)
    {
        foreach (MixerRecipe mixerRecipe in _mixerRecipeList)
        {
            if (IsRecipeMatch(ingredientsInPlate, mixerRecipe))
            {
                return true;
            }
        }

        return false;
    }
    private bool IsRecipeMatch(List<KitchenObjectSO> ingredientsInPlate, MixerRecipe mixerRecipe)
    {
        if (ingredientsInPlate.Count != mixerRecipe.mixerRecipeIngredients.Count)
            return false;

        foreach (KitchenObjectSO ingredient in ingredientsInPlate)
        {
            if (!mixerRecipe.mixerRecipeIngredients.Contains(ingredient))
            {
                return false;
            }
        }

        _mixerTimerMax.Value = mixerRecipe.mixerTimerMax;
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        _state.Value = State.Idle;
        _kitchenObjectSOList.Clear();
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
        _kitchenObjectSOList.Add(kitchenObjectSO);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _kitchenObjectSOList.Add(kitchenObjectSO);
    }
}