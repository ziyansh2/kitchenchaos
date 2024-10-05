using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter {

	public override void Interact(Player player) {
		//There is a kitchen Object
		if (player.HasKitchenObject()) {
			//Player is carrying something
			if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
				//Player is holding a Plate
				player.GetKitchenObject().DestroySelf();
			} 
		}
	}

}
