using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour {

    public const int MAX_PLAYER_AMOUNT = 4;
    public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLYER = "PlayerNameMultiplayer";

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    public static KitchenGameMultiplayer Instance { get; private set; }
    public static bool playMultiplayer = true;
    
    public int NetworkManager_Client_OnClientConnectCallback { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColorList;

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));

        //Have to initialize here, or it will cause stack leak.
        playerDataNetworkList = new();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void Start() {
        if (playMultiplayer) { 
        
        } else {
            StartHost();
            Loader.LoadNetwork(Loader.Scene.SC_Multiplayer);
        }
    }

    public string GetPlayerName() { 
        return playerName; 
    }

    public void SetPlayerName(string playerName) {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLYER, this.playerName);
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientID) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientID == clientID) {
                playerDataNetworkList.RemoveAt(i);
                return;
            }
        }
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientID) {
        playerDataNetworkList.Add(new PlayerData() {
            clientID = clientID,
            colorID = GetFirstUnusedColorID(),
            playerName = GetPlayerName(),
            playerID = AuthenticationService.Instance.PlayerId
        });
    }

    public void StartClient() {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientID) {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerID, ServerRpcParams serverRpcParams = default) {
        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerID = playerID;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID) {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest arg1, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
        if (SceneManager.GetActiveScene().name != Loader.Scene.SC_CharacterSelect.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKichenObjectParent kichenObjectParent) {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kichenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectFromIndex(kitchenObjectSOIndex);

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKichenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKichenObjectParent>();
        if (kitchenObjectParent.HasKitchenObject()) return;

        KitchenObject kitchenObject = Instantiate(kitchenObjectSO.prefab).GetComponent<KitchenObject>();

        NetworkObject kitchenObjNetworkObj = kitchenObject.GetComponent<NetworkObject>();
        kitchenObjNetworkObj.Spawn(true);

        kitchenObject?.SetKitchenObjectParent(kitchenObjectParent);
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject) {
        kitchenObject.ClearKitchenObjectOnParent();
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        if (kitchenObjectNetworkObject == null) return;

        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObject);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
        kitchenObject.ClearKitchenObjectOnParent();
    }

    public int GetPlayerDataIndexFromClientID(ulong clientID) {
        for (int i = 0; i < playerDataNetworkList.Count; i++) {
            if (playerDataNetworkList[i].clientID == clientID)
                return i;
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientID(ulong clientID) {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.clientID == clientID)
                return playerData;
        }
        return default;
    }

    public PlayerData GetPlayerData() {
        return GetPlayerDataFromClientID(NetworkManager.Singleton.LocalClientId);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectFromIndex(int kithenObjectSoIndex) {
        return kitchenObjectListSO.kitchenObjectSOList[kithenObjectSoIndex];
    }

    public bool IsPlayerIndexConnected(int playerIndex) {
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorID) {
        return playerColorList[colorID];
    }

    public void ChangePlayerColor(int colorID) {
        ChangePlayerColorServerRpc(colorID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorID, ServerRpcParams serverRpcParams = default) {
        if (!IsColorAvailable(colorID)) {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientID(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorID = colorID;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorID) {
        foreach (PlayerData playerData in playerDataNetworkList) {
            if (playerData.colorID == colorID)
                return false;
        }
        return true;
    }

    private int GetFirstUnusedColorID() {
        for (int i = 0; i < playerColorList.Count; i++) {
            if (IsColorAvailable(i))
                return i;
        }
        return -1;
    }

    public void KickPlayer(ulong clientID) {
        NetworkManager.Singleton.DisconnectClient(clientID);
        NetworkManager_Server_OnClientDisconnectCallback(clientID);
    }

}
