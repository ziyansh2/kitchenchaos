using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour {

    [SerializeField] int playerIndex;
    [SerializeField] GameObject readyGameObject;
    [SerializeField] PlayerVisual playerVisual;

    private void Start() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void UpdatePlayer() {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();
            bool isPlayerReady = CharacterSelectReady.Instance.IsPlayerReady(KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex).clientID);
            readyGameObject.SetActive(isPlayerReady);
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerIndex));
        } else { 
            Hide();
            readyGameObject.SetActive(false);
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
