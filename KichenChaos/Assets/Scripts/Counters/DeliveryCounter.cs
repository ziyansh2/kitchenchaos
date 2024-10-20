using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter {

	public static DeliveryCounter Instance { get; private set; }

	private void Awake() {
		Instance = this;
	}

	public override void Interact(Player player) {
		//There is a kitchen Object
		if (player.HasKitchenObject()) {
			//Player is carrying something
			if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
				//Player is holding a Plate

				DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
				KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
			} 
		}
	}

}
