using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour {

    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake() {
        startHostButton.onClick.AddListener(()=> {
            Debug.Log("Host");
            NetworkManager.Singleton.StartHost();
            KitchenGameManager.Instance.DebugStart();
            Hide();
        });
        startClientButton.onClick.AddListener(() => {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
            KitchenGameManager.Instance.DebugStart();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}
