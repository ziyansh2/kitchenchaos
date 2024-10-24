using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlatePicked;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update() {
        if (!IsServer) return;
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer >= spawnPlateTimerMax) {
            spawnPlateTimer = 0;
            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            }
        }
	}

	public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            if (platesSpawnedAmount > 0) {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc() {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlatePicked?.Invoke(this, EventArgs.Empty);
        platesSpawnedAmount--;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlateServerRpc() {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc() {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

}
