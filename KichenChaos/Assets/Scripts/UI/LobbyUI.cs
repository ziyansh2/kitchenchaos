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

    private void Awake() {
        mainMenuButton.onClick.AddListener(() => {
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


}
