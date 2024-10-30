using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

	public static event EventHandler OnAnyCut;

	new public static void ResetStaticData() {
		OnAnyCut = null;
	}

	public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

	public event EventHandler OnCut;

	[SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

	private int cuttingProgress;

	public override void Interact(Player player) {
		if (!HasKitchenObject()) {
			//There is no kitchen Object
			if (player.HasKitchenObject()) {
				//Player is carrying something
				if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
					//Drop it to the counter
					player.GetKitchenObject().SetKitchenObjectParent(this);
					ResetCounterProgressServerRpc();
				}
			}
		} else {
			//There is a kitchen Object
			if (player.HasKitchenObject()) {
				//Player is carrying something
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					//Player is holding a Plate
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
						KitchenObject.DestroyKitchenObject(GetKitchenObject());
						ResetCounterProgressServerRpc();
					}
				}
			} else {
				//Player is not carrying anything
				GetKitchenObject().SetKitchenObjectParent(player);
				ResetCounterProgressServerRpc();
			}
		}
	}


	[ServerRpc(RequireOwnership = false)]
	private void ResetCounterProgressServerRpc() {
		ResetCounterProgressOnCounterClientRpc();
	}

	[ClientRpc]
	private void ResetCounterProgressOnCounterClientRpc() {
		cuttingProgress = 0;
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = 0 });
	}

	public override void OnInteractAlternate(Player player) {
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
			CutObjectServerRpc();
			TestCuttingProgressDoneServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void CutObjectServerRpc() {
		CutObjectClientRpc();
	}

	[ClientRpc]
	private void CutObjectClientRpc() {
		cuttingProgress++;
		OnCut?.Invoke(this, EventArgs.Empty);
		OnAnyCut?.Invoke(this, EventArgs.Empty);

		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
		OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
			progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
		});
	}

	[ServerRpc(RequireOwnership = false)]
	private void TestCuttingProgressDoneServerRpc() {
		CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
		if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
			KitchenObject.DestroyKitchenObject(GetKitchenObject());
			KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, this);
		}
	}

	private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObejectSO) {
		foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
			if (cuttingRecipeSO.input == inputKitchenObejectSO) {
				return cuttingRecipeSO;
			}
		}
		return null;
	}

	private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObejectSO) {
		return GetCuttingRecipeSOWithInput(inputKitchenObejectSO) != null;
	}

	private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObejectSO) {
		return GetCuttingRecipeSOWithInput(inputKitchenObejectSO)?.output;
	}

}
