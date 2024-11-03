using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMassageUI : MonoBehaviour
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
        Hide();
    }

    void OnDestroy() {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e) {
        Show();
        messageText.text = NetworkManager.Singleton.DisconnectReason;
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}
