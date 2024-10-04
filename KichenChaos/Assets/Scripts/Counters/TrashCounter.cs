using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter {

    public override void Interact(Player player) {
		//Player is carrying something
		if (player.HasKitchenObject()) {
            player.GetKitchenObject().DestroySelf();
        }
    }

}
