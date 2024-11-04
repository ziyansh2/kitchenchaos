using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMPro.TextMeshProUGUI lobbyNameText;
    [SerializeField] private TMPro.TextMeshProUGUI lobbyCodeText;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.SC_MainMenu);
        });
        readyButton.onClick.AddListener(() => {
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start() {
        Lobby joinedLoby = KitchenGameLobby.Instance.GetLobby();
        lobbyNameText.text ="Lobby Name:" + joinedLoby.Name;
        lobbyCodeText.text ="Lobby Code:" + joinedLoby.LobbyCode;
    }

}
