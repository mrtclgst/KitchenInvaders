using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyStatusChanged;

    private Dictionary<ulong, bool> _playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetReadyPlayer()
    {
        SetReadyPlayerServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetReadyPlayerClientRpc(serverRpcParams.Receive.SenderClientId);
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allPlayersReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientID) || !_playerReadyDictionary[clientID])
            {
                //this player is not ready
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady)
        {
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetworkScene(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetReadyPlayerClientRpc(ulong clientId)
    {
        _playerReadyDictionary[clientId] = true;
        OnReadyStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
    }
}