using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour {

	public static KitchenGameManager Instance { get; private set; }

	public event EventHandler OnStateChanged;


	private enum State {
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	private State state;

	private float waitingToStartTimer = 1f;
	private float countdownToStartTimer = 3f;
	private float gameplayingTimer;
	private float gameplayingTimerMax = 10f;

	private void Awake() {
		Instance = this;
		state = State.WaitingToStart;
	}

	private void Update() {
		Debug.Log(state);
		switch (state) {
			case State.WaitingToStart:
				waitingToStartTimer -= Time.deltaTime;
				if (waitingToStartTimer < 0) {
					state = State.CountdownToStart;
					OnStateChanged?.Invoke(this, EventArgs.Empty);
				}
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

}

