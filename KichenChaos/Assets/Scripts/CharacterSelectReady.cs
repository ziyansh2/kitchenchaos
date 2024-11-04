using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour {

    public static CharacterSelectReady Instance { get; private set; }


    public event EventHandler OnReadyChanged;
    private Dictionary<ulong, bool> playerReadyDictionary = new();

    private void Awake() {
        Instance = this;
    }

    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(clientID) || playerReadyDictionary[clientID] == false) return;
        }

        //All players ready
        KitchenGameLobby.Instance.DeleteLobby();
        Loader.LoadNetwork(Loader.Scene.SC_Multiplayer);
    }

    [ClientRpc(RequireOwnership = false)]
    private void SetPlayerReadyClientRpc(ulong clientID) {
        playerReadyDictionary[clientID] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID) {
        if(playerReadyDictionary.ContainsKey(clientID))
            return playerReadyDictionary[clientID];
        return false;
    }
}
