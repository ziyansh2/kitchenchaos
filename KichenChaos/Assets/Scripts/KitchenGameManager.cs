using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {

	public static KitchenGameManager Instance { get; private set; }

	public event EventHandler OnStateChanged;
	public event EventHandler OnPaused;
	public event EventHandler OnUnpaused;

	private enum State {
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	private State state;

	private float countdownToStartTimer = 1f;
	private float gameplayingTimer;
	private float gameplayingTimerMax = 300f;
	private bool isGamePaused = false;

	private void Awake() {
		Instance = this;
		state = State.WaitingToStart;
	}

	private void Start() {
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;



	}

	public void DebugStart() {
		//Debug trigger game start automatically
		state = State.CountdownToStart;
		OnStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void GameInput_OnInteractAction(object sender, EventArgs e) {
		if (state == State.WaitingToStart) {
			state = State.CountdownToStart;
			OnStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void GameInput_OnPauseAction(object sender, EventArgs e) {
		TogglePauseGame();
	}

	private void Update() {
		switch (state) {
			case State.WaitingToStart:
				break;
			case State.CountdownToStart:
				countdownToStartTimer -= Time.deltaTime;
				if (countdownToStartTimer < 0) {
					state = State.GamePlaying;
					gameplayingTimer = gameplayingTimerMax;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case State.GamePlaying:
				gameplayingTimer -= Time.deltaTime;
				if (gameplayingTimer < 0) {
					state = State.GameOver;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
				break;
			case State.GameOver:
				break;
		}
	}

	public bool IsGamePlaying() {
		return state == State.GamePlaying;
	}

	public bool IsCountdownToStartActive() {
		return state == State.CountdownToStart;
	}

	public bool IsGameOver() {
		return state == State.GameOver;
	}

	public float GetCountdownToStartTimer() {
		return countdownToStartTimer;
	}

	public float GetGamePlayingTimerNormalized() {
		return 1 - gameplayingTimer / gameplayingTimerMax;
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

