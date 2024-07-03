using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler Event_OnPlateSpawned;
    public event EventHandler Event_OnPlateTaken;

    [SerializeField] private KitchenObjectSO _plateKitchenObjectSO;
    [SerializeField] private float _timeToSpawn = 4f;
    [SerializeField] int _maxPlatesCount = 4;

    private float _spawnTimer = 0f;
    private int _spawnedPlatesCount = 0;

    private void Update()
    {
        if (!IsServer)
            return;

        if (_spawnedPlatesCount >= _maxPlatesCount)
            return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= _timeToSpawn)
        {
            _spawnTimer = 0f;
            SpawnPlateServerRpc();
        }
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {

        }
        else
        {
            KitchenObject.CreateKitchenObject(_plateKitchenObjectSO, player);
            InteractLogicServerRpc();
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        _spawnedPlatesCount++;
        Event_OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        Event_OnPlateTaken?.Invoke(this, EventArgs.Empty);
        _spawnedPlatesCount--;
    }
}