using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler Event_OnRecipeSpawned;
    public event EventHandler Event_OnRecipeDelivered;
    public event EventHandler Event_OnRecipeSuccess;
    public event EventHandler Event_OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO _recipeListSO;
    [SerializeField] private float _timeToSpawnRecipe = 4f;
    [SerializeField] private int _maxRecipeCount = 4;
    private List<RecipeSO> _waitingSOList;
    private float _spawnRecipeTimer;
    private int _successfullyDeliveredCount;

    private void Awake()
    {
        _waitingSOList = new List<RecipeSO>();
        Instance = this;
    }
    private void Update()
    {
        if (!IsServer)
            return;

        if (!GameManager.Instance.IsGamePlaying() || _waitingSOList.Count >= _maxRecipeCount)
            return;

        _spawnRecipeTimer += Time.deltaTime;
        if (_spawnRecipeTimer >= _timeToSpawnRecipe)
        {
            int recipeSOIndex = UnityEngine.Random.Range(0, _recipeListSO.GetRecipeSOList().Count);
            SpawnNewWaitingRecipeClientRpc(recipeSOIndex);
            _spawnRecipeTimer = 0;
        }
    }
    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int recipeSOIndex)
    {
        RecipeSO recipeSO = _recipeListSO.GetRecipeAtIndex(recipeSOIndex);
        _waitingSOList.Add(recipeSO);
        Event_OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }


    internal void DeliverPlate(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < _waitingSOList.Count; i++)
        {
            RecipeSO recipeSO = _waitingSOList[i];

            var set = new HashSet<KitchenObjectSO>(recipeSO.GetKitchenObjectSOList());
            bool equals = set.SetEquals(plateKitchenObject.GetKitchenObjectSOList());
            if (equals)
            {
                DeliverCorrectRecipeServerRpc(i);
                return;
            }
        }
        DeliverInCorrectRecipeServerRpc();
    }



    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIndex)
    {
        _successfullyDeliveredCount++;
        _waitingSOList.RemoveAt(waitingRecipeSOIndex);
        Event_OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        Event_OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverInCorrectRecipeServerRpc()
    {
        DeliverInCorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverInCorrectRecipeClientRpc()
    {
        Event_OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingSOList()
    {
        return _waitingSOList;
    }
    public int GetSuccessfullyDeliveredCount()
    {
        return _successfullyDeliveredCount;
    }
}
