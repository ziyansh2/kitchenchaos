using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {

    [SerializeField] int playerIndex;
    [SerializeField] GameObject readyGameObject;
    [SerializeField] PlayerVisual playerVisual;
    [SerializeField] Button kickButton;

    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientID);
        });
    }

    private void Start() {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton)
            KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
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

            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            bool isPlayerReady = CharacterSelectReady.Instance.IsPlayerReady(playerData.clientID);
            readyGameObject.SetActive(isPlayerReady);
            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorID));
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
