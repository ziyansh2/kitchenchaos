using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {

	[SerializeField] Button resumeButton;
	[SerializeField] Button OptionsButton;
	[SerializeField] Button mainMenuButton;

	private void Awake() {
		resumeButton.onClick.AddListener(() => {
			KitchenGameManager.Instance.TogglePauseGame();
		});

		OptionsButton.onClick.AddListener(() => {
			Hide();
			OptionsUI.Instance.Show(Show);
		});

		mainMenuButton.onClick.AddListener(() => {
			Loader.Load(Loader.Scene.SC_MainMenuScene);
		});
	}

	private void Start() {
		KitchenGameManager.Instance.OnLocalGamePaused += KitchenGameManager_OnLocalGamePaused;
		KitchenGameManager.Instance.OnLocalGameUnpaused += KitchenGameManager_OnLocalGameUnpaused;

		Hide();
	}

	private void KitchenGameManager_OnLocalGameUnpaused(object sender, System.EventArgs e) {
		Hide();
	}

	private void KitchenGameManager_OnLocalGamePaused(object sender, System.EventArgs e) {
		Show();
	}

	private void Show() {
		gameObject.SetActive(true);

		resumeButton.Select();
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

}
