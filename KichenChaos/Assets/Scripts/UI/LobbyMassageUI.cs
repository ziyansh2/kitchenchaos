using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMassageUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        Hide();
    }
    void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
    }


    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs e) {
        ShowMessage("Failed to join lobby!");
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs e) {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, EventArgs e) {
        ShowMessage("Joining Lobby...");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs e) {
        ShowMessage("Creating Lobby...");
    }

    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs e) {
        ShowMessage("Failed to create Lobby!");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e) {
        if (NetworkManager.Singleton.DisconnectReason == "") {
            ShowMessage("Failed to connect");
        } else {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message) {
        Show();
        messageText.text = message;
    }


    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}
