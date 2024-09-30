using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            //There is no kitchen Object
            if (player.HasKitchenObject()) {
                //Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        } else {
            //There is a kitchen Object
            if (!player.HasKitchenObject()) {
                //Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
