using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {

	[SerializeField] Button resumeButton;
	[SerializeField] Button mainMenuButton;

	private void Awake() {
		resumeButton.onClick.AddListener(() => {
			KitchenGameManager.Instance.TogglePauseGame();
		});

		mainMenuButton.onClick.AddListener(() => {
			Loader.Load(Loader.Scene.SC_MainMenuScene);
		});
	}

	private void Start() {
		KitchenGameManager.Instance.OnPaused += KitchenGameManager_OnPaused;
		KitchenGameManager.Instance.OnUnpaused += KitchenGameManager_OnUnpaused;

		Hide();
	}

	private void KitchenGameManager_OnUnpaused(object sender, System.EventArgs e) {
		Hide();
	}

	private void KitchenGameManager_OnPaused(object sender, System.EventArgs e) {
		Show();
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

}
