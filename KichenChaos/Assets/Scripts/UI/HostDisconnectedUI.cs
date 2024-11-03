using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour {

    [SerializeField] private Button playAgainButton;

    private void Awake() {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.SC_MainMenu);
        });
    }

    private void Start() {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
        Hide();
    }

    private void NetworkManager_OnClientDisconnectedCallback(ulong clientID) {
        if (clientID == NetworkManager.ServerClientId) {
            //Server is shutting down
            Show();
        }

        //Has an engine bug that the clientID sent from the system is wrong
        //Show the menu what ever the client id is
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
