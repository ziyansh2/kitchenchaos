using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour {

    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;

    private void Awake() {
        startHostButton.onClick.AddListener(()=> {
            KitchenGameMultiplayer.Instance.StartHost();
            Hide();
        });
        startClientButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.Instance.StartClient();
            Hide();
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}
