using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

	public static event EventHandler OnAnyCut;

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
					cuttingProgress = 0;
					OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() { progressNormalized = 0 });
				}
			}
		} else {
			//There is a kitchen Object
			if (player.HasKitchenObject()) {
				//Player is carrying something
				if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
					//Player is holding a Plate
					if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
						GetKitchenObject().DestroySelf();
						OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
							progressNormalized = 0
						});
					}
				}
			} else {
				//Player is not carrying anything
				GetKitchenObject().SetKitchenObjectParent(player);
				OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
					progressNormalized = 0
				});
			}
		}
	}

	public override void OnInteractAlternate(Player player) {
		if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
			cuttingProgress++;
			OnCut?.Invoke(this, EventArgs.Empty);
			OnAnyCut?.Invoke(this, EventArgs.Empty);

			CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
			OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
				progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
			});
			if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
				GetKitchenObject().DestroySelf();
				KitchenObject.SpawnKitchenObject(cuttingRecipeSO.output, this);
			}
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
