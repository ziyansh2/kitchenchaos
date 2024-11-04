using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button quickJoinGameButton;
    
    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.SC_MainMenu);
        });
        createGameButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreateLobbyAsync("test", false);
        });
        quickJoinGameButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.QuickJoin();
        });
    }


}
