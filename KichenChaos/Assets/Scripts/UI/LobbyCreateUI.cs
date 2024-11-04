using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{

    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMPro.TMP_InputField lobbyNameInputField;

    private void Awake() {
        createPublicButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreateLobbyAsync(lobbyNameInputField.text, false);
        });
        createPrivateButton.onClick.AddListener(() => {
            KitchenGameLobby.Instance.CreateLobbyAsync(lobbyNameInputField.text, true);
        });
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}
