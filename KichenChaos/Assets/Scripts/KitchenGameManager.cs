using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour {

	public static KitchenGameManager Instance { get; private set; }

	public event EventHandler OnStateChanged;
	public event EventHandler OnPaused;
	public event EventHandler OnUnpaused;
	public event EventHandler OnLocalPlayerReadyChange;

	private enum State {
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	private NetworkVariable<State> state = new(State.WaitingToStart);
	private bool isLocalPlayerReady = false;
	private NetworkVariable<float> countdownToStartTimer = new(3f);
	private NetworkVariable<float> gameplayingTimer = new();
	private float gameplayingTimerMax = 300f;
	private bool isGamePaused = false;
	private Dictionary<ulong, bool> playerReadyDictionary = new();

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
	}

	public override void OnNetworkSpawn() {
		state.OnValueChanged += State_OnValueChanged;
	}

    private void State_OnValueChanged(State previousValue, State newValue) {
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
		if (state.Value == State.WaitingToStart) {
			isLocalPlayerReady = true;
			SetPlayerReadyServerRpc();
			OnLocalPlayerReadyChange?.Invoke(this, EventArgs.Empty);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParam = default) {
		playerReadyDictionary[serverRpcParam.Receive.SenderClientId] = true;

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

	public bool IsGamePlaying() {
		return state.Value == State.GamePlaying;
	}

	public bool IsCountdownToStartActive() {
		return state.Value == State.CountdownToStart;
	}

	public bool IsGameOver() {
		return state.Value == State.GameOver;
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
		isGamePaused = !isGamePaused;
		if (isGamePaused) {
			Time.timeScale = 0f;
			OnPaused?.Invoke(this, EventArgs.Empty);
		} else {
			Time.timeScale = 1f;
			OnUnpaused?.Invoke(this, EventArgs.Empty);
		}

	}

}

