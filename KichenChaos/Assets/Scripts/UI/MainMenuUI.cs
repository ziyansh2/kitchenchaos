using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

	[SerializeField] private Button playMultiplayerButton;
	[SerializeField] private Button playSinglePlayerButton;
	[SerializeField] private Button quitButton;

	private void Awake() {
		playMultiplayerButton.onClick.AddListener(() => {
			KitchenGameMultiplayer.playMultiplayer = true;
			Loader.Load(Loader.Scene.SC_Lobby);
		});
		playSinglePlayerButton.onClick.AddListener(() => {
			KitchenGameMultiplayer.playMultiplayer = false;
			Loader.Load(Loader.Scene.SC_Lobby);
		});
		quitButton.onClick.AddListener(() => {
			Application.Quit();
		});

		Time.timeScale = 1f;
	}

}

