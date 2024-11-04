using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class KitchenGameLobby : MonoBehaviour {

    static public KitchenGameLobby Instance;

    public EventHandler OnCreateLobbyStarted;
    public EventHandler OnCreateLobbyFailed;
    public EventHandler OnJoinStarted;
    public EventHandler OnQuickJoinFailed;
    public EventHandler OnJoinFailed;

    public EventHandler<OnLobbyLIstChangeEventArgs> OnLobbyListChanged;
    public class OnLobbyLIstChangeEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthenticationAsync();
    }

    private void Update() {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies() {
        if (joinedLobby != null) return;
        if (!AuthenticationService.Instance.IsSignedIn) return;

        listLobbiesTimer -= Time.deltaTime;
        if (listLobbiesTimer <= 0) {
            float listLobbiesTimerMax = 3f;
            listLobbiesTimer = listLobbiesTimerMax;
            ListLobbies();
        }
    }

    private void HandleHeartbeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitializeUnityAuthenticationAsync() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async void ListLobbies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new() {
                Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }};

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyLIstChangeEventArgs { lobbyList = queryResponse.Results });

        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }   
    }


    public async void CreateLobby(string lobbyName, bool isPrivate) {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(
                lobbyName,
                KitchenGameMultiplayer.MAX_PLAYER_AMOUNT,
                new CreateLobbyOptions { IsPrivate = isPrivate }
            );

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.SC_CharacterSelect);

        } catch (LobbyServiceException e) {
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public async void QuickJoin() {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            KitchenGameMultiplayer.Instance.StartClient();

        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithCode(string lobbyCode) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            KitchenGameMultiplayer.Instance.StartClient();

        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithID(string lobbyID) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);

            KitchenGameMultiplayer.Instance.StartClient();

        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;

            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;

            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerID) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerID);

            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    public Lobby GetLobby() {
        return joinedLobby;
    }

}
