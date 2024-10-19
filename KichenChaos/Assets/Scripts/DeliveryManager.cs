using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour {

	public event EventHandler OnRecipeSpawned;
	public event EventHandler OnRecipeCompleted;
	public event EventHandler OnRecipeSuccess;
	public event EventHandler OnRecipeFailed;

	public static DeliveryManager Instance { get; private set; }

	[SerializeField] private RecipeListSO recipeListSO;

	private List<RecipeSO> waitingRecipeSOList = new();
	private float spawnRecipeTimer = 4f;
	private float spawnRecipeTimerMax = 4f;
	private int waitingRecipesMax = 4;
	private int successfulRecipesAmount;

	private void Awake() {
		Instance = this;
	}

	private void Update() {
		if (!IsServer) return;

		spawnRecipeTimer -= Time.deltaTime;
		if (spawnRecipeTimer <= 0) {
			spawnRecipeTimer = spawnRecipeTimerMax;

			if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax) {
				int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
				SpawnNetWaitingRecipeClientRpc(waitingRecipeSOIndex);
			}
		}
	}

	[ClientRpc]
	private void SpawnNetWaitingRecipeClientRpc(int waitingRecipeSOIndex) {
		RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
		waitingRecipeSOList.Add(waitingRecipeSO);
		OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
	}

	public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
		for (int i = 0; i < waitingRecipeSOList.Count; i++) {
			RecipeSO waitingRecipeSO = waitingRecipeSOList[i];
			if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
				//Has the same number of ingredients
				bool plateContentMatchesRecipe = true;
				foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
					//Cycling trhough all ingredients in the Recipe
					if (!plateKitchenObject.GetKitchenObjectSOList().Contains(recipeKitchenObjectSO)) {
						//This Recipe ingredient was not found on the Plate
						plateContentMatchesRecipe = false;
						return;
					}
				}

				if (plateContentMatchesRecipe) {
					//Player delivered the correct recipe
					DeliverCorrectReceipeServerRpc(i);
					return;
				}
			}
		}

		//No matches found
		DeliverIncorrectReceipeServerRpc();
	}


	[ServerRpc(RequireOwnership = false)]
	private void DeliverIncorrectReceipeServerRpc() {
		DeliverIncorrectReceipeClientRpc();
	}

	[ClientRpc]
	private void DeliverIncorrectReceipeClientRpc() {
		OnRecipeFailed?.Invoke(this, EventArgs.Empty);
	}


	[ServerRpc(RequireOwnership = false)]
	private void DeliverCorrectReceipeServerRpc(int waitingRecipeSOListIndex) {
		DeliverCorrectReceipeClientRpc(waitingRecipeSOListIndex);
	}

	[ClientRpc]
	private void DeliverCorrectReceipeClientRpc(int waitingRecipeSOListIndex) {
		waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);
		OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
		OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
		successfulRecipesAmount++;
	}

	public List<RecipeSO> GetWaitingRecipeSOList() {
		return waitingRecipeSOList;
	}

	public int GetSuccessfulRecipesAmount() {
		return successfulRecipesAmount;
	}

}
