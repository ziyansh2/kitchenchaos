using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button quickJoinGameButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMPro.TMP_InputField lobbyCodeInputField;
    [SerializeField] private TMPro.TMP_InputField playerNameInputField;

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.SC_MainMenu);
        });
        createGameButton.onClick.AddListener(() => {
            lobbyCreateUI.Show();
        });
        quickJoinGameButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.JoinWithCode(lobbyCodeInputField.text);
        });
    }

    private void Start() {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string value) => {
            KitchenGameMultiplayer.Instance.SetPlayerName(value);
        });
    }

}
