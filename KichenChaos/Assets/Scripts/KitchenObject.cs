using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKichenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKichenObjectParent kichenObjectParent) {
        if (kitchenObjectParent != null) {
            kitchenObjectParent.ClearKitchenObject();
        }

        kitchenObjectParent = kichenObjectParent;
        if (kichenObjectParent.HasKitchenObject()) {
            Debug.LogError("Counter already has a KitchenObject!");
        }
        kitchenObjectParent.SetKitchenObject(this);

        //transform.parent = kichenObjectParent.GetKitchenObjectFollowTransform();
        //transform.localPosition = Vector3.zero;
    }

    public IKichenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

    public void DestroySelf() {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKichenObjectParent kichenObjectParent) {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kichenObjectParent);
    }

}
