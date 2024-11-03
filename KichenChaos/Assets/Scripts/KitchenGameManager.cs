using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour {

	public static KitchenGameManager Instance { get; private set; }

	public event EventHandler OnStateChanged;
	public event EventHandler OnLocalGamePaused;
	public event EventHandler OnLocalGameUnpaused;
	public event EventHandler OnMultiplayerGamePaused;
	public event EventHandler OnMultiplayerGameUnpaused;
	public event EventHandler OnLocalPlayerReadyChange;

	private enum State {
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	[SerializeField] private Transform playerPrefab;

	private NetworkVariable<State> state = new(State.WaitingToStart);
	private bool isLocalPlayerReady = false;
	private NetworkVariable<float> countdownToStartTimer = new(3f);
	private NetworkVariable<float> gameplayingTimer = new();
	private float gameplayingTimerMax = 90f;
	private bool isLocaclGamePaused = false;
	private NetworkVariable<bool> isGamePaused = new();
	private Dictionary<ulong, bool> playerReadyDictionary = new();
	private Dictionary<ulong, bool> playerPauseDictionary = new();
	private bool autoTestGamePausedState;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
	}

	public override void OnNetworkSpawn() {
		state.OnValueChanged += State_OnValueChanged;
		isGamePaused.OnValueChanged += IsGamePause_OnValueChanged;

		if (IsServer) {
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkSceneManager_OnLoadEventCompleted;
		}
	}

    private void NetworkSceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
		foreach (ulong clientID in clientsCompleted) {
			Transform playerTransform = Instantiate(playerPrefab);
			playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
		}
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID) {
		autoTestGamePausedState = true;
    }

    private void IsGamePause_OnValueChanged(bool previousValue, bool newValue) {
		if (isGamePaused.Value) {
			Time.timeScale = 0;
			OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
		} else {
			Time.timeScale = 1;
			OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
		}
    }

    private void State_OnValueChanged(State previousValue, State newValue) {
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
		if (state.Value == State.WaitingToStart) {
			isLocalPlayerReady = true;
			OnLocalPlayerReadyChange?.Invoke(this, EventArgs.Empty);
			SetPlayerReadyServerRpc();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds) {
			if (!playerReadyDictionary.ContainsKey(clientID) || playerReadyDictionary[clientID] == false) return;
        }
		state.Value = State.CountdownToStart;
	}

	private void GameInput_OnPauseAction(object sender, EventArgs e) {
		TogglePauseGame();
	}

	private void Update() {
		if (!IsServer) return;

		switch (state.Value) {
			case State.WaitingToStart:
				break;
			case State.CountdownToStart:
				countdownToStartTimer.Value -= Time.deltaTime;
				if (countdownToStartTimer.Value < 0) {
					state.Value = State.GamePlaying;
					gameplayingTimer.Value = gameplayingTimerMax;
				}
				break;
			case State.GamePlaying:
				gameplayingTimer.Value -= Time.deltaTime;
				if (gameplayingTimer.Value < 0) {
					state.Value = State.GameOver;
				}
				break;
			case State.GameOver:
				break;
		}
	}

    private void LateUpdate() {
		if (autoTestGamePausedState) {
			autoTestGamePausedState = false;
			TestGamePausedState();
		}
	}

    public bool IsGamePlaying() {
		return state.Value == State.GamePlaying;
	}

	public bool IsCountdownToStartActive() {
		return state.Value == State.CountdownToStart;
	}

	public bool IsGameOver() {
		return state.Value == State.GameOver;
	}

	public bool IsWaitingToStart() {
		return state.Value == State.WaitingToStart;
	}

	public float GetCountdownToStartTimer() {
		return countdownToStartTimer.Value;
	}

	public float GetGamePlayingTimerNormalized() {
		return 1 - gameplayingTimer.Value / gameplayingTimerMax;
	}

	public bool IsLocalPlayerReady() {
		return isLocalPlayerReady;
	}
	public void TogglePauseGame() {
		isLocaclGamePaused = !isLocaclGamePaused;
		if (isLocaclGamePaused) {
			PauseGameServerRpc();
			OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
		} else {
			UnpauseGameServerRpc();
			OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
		playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
		TestGamePausedState();
	}

	[ServerRpc(RequireOwnership = false)]
	private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default) {
		playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
		TestGamePausedState();
	}

	private void TestGamePausedState() {
		foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds) {
			if (playerPauseDictionary.ContainsKey(clientID) && playerPauseDictionary[clientID]) {
				//This player is paused
				isGamePaused.Value = true;
				return;
			}
		}
		//All players are unpaused
		isGamePaused.Value = false;
	}

}

