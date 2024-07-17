using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static MixerCounter;

public class OvenCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.Event_OnProgressChangedArgs> Event_OnProgressChanged;
    public event EventHandler<Event_OnStateChangedArgs> Event_OnStateChanged;
    [Serializable]
    public struct OvenRecipe
    {
        public List<KitchenObjectSO> ovenRecipeIngredients;
        public float ovenTimerMax;
    }
    public class Event_OnStateChangedArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Baking,
    }

    [SerializeField] private List<OvenRecipe> _ovenRecipeList;
    [SerializeField] private List<KitchenObjectSO> _kitchenObjectSOList;
    [SerializeField] private KitchenObjectSO _bakedKitchenObjectSO;

    private NetworkVariable<float> _bakingTimer = new NetworkVariable<float>();
    private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> _bakingTimerMax = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        _bakingTimer.OnValueChanged += BakingTimer_OnValueChanged;
        _state.OnValueChanged += State_OnValueChanged;
    }
    private void BakingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float bakingTimerMax = _bakingTimerMax.Value != 0 ? _bakingTimerMax.Value : 1f;
        Event_OnProgressChanged?.Invoke(this, new IHasProgress.Event_OnProgressChangedArgs
        { ProgressNormalized = _bakingTimer.Value / bakingTimerMax });
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

    private void Update()
    {
        if (!IsServer) { return; }
        //if (HasKitchenObject())
        //{
        switch (_state.Value)
        {
            case State.Idle:
                break;
            case State.Baking:
                _bakingTimer.Value += Time.deltaTime;
                if (_bakingTimer.Value > _bakingTimerMax.Value)
                {
                    //KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    //KitchenObject.CreateKitchenObject(_mixedKitchenObject, this);
                    SetMixingRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(_bakedKitchenObjectSO));
                    _state.Value = State.Idle;
                    _bakingTimer.Value = 0;
                }
                break;
            default:
                break;
        }
        //}
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
        foreach (OvenRecipe ovenRecipe in _ovenRecipeList)
        {
            if (IsRecipeMatch(ingredientsInPlate, ovenRecipe))
            {
                return true;
            }
        }

        return false;
    }
    private bool IsRecipeMatch(List<KitchenObjectSO> ingredientsInPlate, OvenRecipe ovenRecipe)
    {
        if (ingredientsInPlate.Count != ovenRecipe.ovenRecipeIngredients.Count)
            return false;

        foreach (KitchenObjectSO ingredient in ingredientsInPlate)
        {
            if (!ovenRecipe.ovenRecipeIngredients.Contains(ingredient))
            {
                return false;
            }
        }

        _bakingTimerMax.Value = ovenRecipe.ovenTimerMax;
        return true;
    }
    public bool IsBaking()
    {
        return _state.Value == State.Baking;
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
        _bakingTimer.Value = 0;
        _state.Value = State.Baking;
        SetMixingRecipeSOClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void SetMixingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        _kitchenObjectSOList.Add(kitchenObjectSO);
    }
}