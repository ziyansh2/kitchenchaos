using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLobbyUI : MonoBehaviour {

    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake() {
        createGameButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.SC_CharacterSelect);
        });
        joinGameButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.StartClient();
        });
    }

}
