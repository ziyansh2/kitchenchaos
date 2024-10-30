using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

	public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
	public class OnIngredientAddedEventArgs : EventArgs {
		public KitchenObjectSO kitchenObjectSO;
	}

	[SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

	private List<KitchenObjectSO> kitchenObjectSOList = new();


	public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
		if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
			//Not a valid ingredient
			return false;
		}
		if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
			return false;
		} else {
			AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
			return true;
		}
	}


	[ServerRpc(RequireOwnership = false)]
	private void AddIngredientServerRpc(int kitchenObjectSOindex) {
		AddIngredientClientRpc(kitchenObjectSOindex);
	}


	[ClientRpc]
	private void AddIngredientClientRpc(int kitchenObjectSOindex) {
		KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectFromIndex(kitchenObjectSOindex);
		kitchenObjectSOList.Add(kitchenObjectSO);
		OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs() { kitchenObjectSO = kitchenObjectSO });
	}


	public List<KitchenObjectSO> GetKitchenObjectSOList() {
		return kitchenObjectSOList;
	}

}
