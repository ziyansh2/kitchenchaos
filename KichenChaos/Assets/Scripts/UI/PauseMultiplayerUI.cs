using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour {

    private void Start() {
        KitchenGameManager.Instance.OnMultiplayerGamePaused += KitchenGameManager_OnMultiplayerPlayerPaused;
        KitchenGameManager.Instance.OnMultiplayerGameUnpaused += KitchenGameManager_OnMultiplayerPlayerUnpaused;

        Hide();
    }

    private void KitchenGameManager_OnMultiplayerPlayerUnpaused(object sender, EventArgs e) {
        Hide();
    }

    private void KitchenGameManager_OnMultiplayerPlayerPaused(object sender, EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
