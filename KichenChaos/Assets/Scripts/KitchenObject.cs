using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour {

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKichenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;

    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKichenObjectParent kitchenObjectParent) {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kichenObjectParentNetworkObjectReference) {
        SetKitchenObjectParentClientRpc(kichenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kichenObjectParentNetworkObjectReference) {
        kichenObjectParentNetworkObjectReference.TryGet(out NetworkObject kichenObjectParentNetworkObject);
        IKichenObjectParent kitchenObjectParent = kichenObjectParentNetworkObject.GetComponent<IKichenObjectParent>();

        //Update old parent
        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;

        //Update new parent
        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("Counter already has a KitchenObject!");
        }
        kitchenObjectParent.SetKitchenObject(this);
        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
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
