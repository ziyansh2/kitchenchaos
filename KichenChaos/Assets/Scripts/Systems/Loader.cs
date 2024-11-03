using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {


	public enum Scene {
		SC_MainMenu,
		SC_Gameplay,
		SC_Loading,
		SC_Lobby,
		SC_CharacterSelect,
		SC_Multiplayer
	}

	private static Scene targetScene;


	public static void Load(Scene targetScene) {
		Loader.targetScene = targetScene;
		SceneManager.LoadScene(Scene.SC_Loading.ToString());
	}

	public static void LoadNetwork(Scene targetScene) {
		NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
	}

	public static void LoadCallback() {
		SceneManager.LoadScene(targetScene.ToString());
	}

}
