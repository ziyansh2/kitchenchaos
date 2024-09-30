using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKichenObjectParent kichenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKichenObjectParent kichenObjectParent) {
        if (this.kichenObjectParent != null) {
            this.kichenObjectParent.ClearKitchenObject();
        }

        this.kichenObjectParent = kichenObjectParent;
        if (kichenObjectParent.HasKitchenObject()) {
            Debug.LogError("Counter already has a KitchenObject!");
        }
        this.kichenObjectParent.SetKitchenObject(this);

        transform.parent = kichenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKichenObjectParent GetKitchenObjectParent() {
        return kichenObjectParent;
    }

}
