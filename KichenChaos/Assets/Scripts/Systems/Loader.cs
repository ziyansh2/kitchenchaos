using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {


	public enum Scene { 
		SC_MainMenuScene,
		SC_Gameplay,
		SC_Loading
	}

	private static Scene targetScene;


	public static void Load(Scene targetScene) {
		Loader.targetScene = targetScene;
		SceneManager.LoadScene(Scene.SC_Loading.ToString());
	}

	public static void LoadCallback() {
		SceneManager.LoadScene(targetScene.ToString());
	}

}
