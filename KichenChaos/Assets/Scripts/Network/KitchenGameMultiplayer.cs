using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour {

    public static KitchenGameMultiplayer Instance { get; private set; }

    public KitchenObjectListSO kitchenObjectListSO;

    private void Awake() {
        Instance = this;
    }


    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKichenObjectParent kichenObjectParent) {        
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kichenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kichenObjectParentNetworkObjectReference) {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectFromIndex(kitchenObjectSOIndex);
        KitchenObject kitchenObject = Instantiate(kitchenObjectSO.prefab).GetComponent<KitchenObject>();
    
        NetworkObject kitchenObjNetworkObj = kitchenObject.GetComponent<NetworkObject>();
        kitchenObjNetworkObj.Spawn(true);

        kichenObjectParentNetworkObjectReference.TryGet(out NetworkObject kichenObjectParentNetworkObject);
        IKichenObjectParent kitchenObjectParent = kichenObjectParentNetworkObject.GetComponent<IKichenObjectParent>();
        kitchenObject?.SetKitchenObjectParent(kitchenObjectParent);
    }

    private int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectFromIndex(int kithenObjectSoIndex) {
        return kitchenObjectListSO.kitchenObjectSOList[kithenObjectSoIndex];
    }

}
