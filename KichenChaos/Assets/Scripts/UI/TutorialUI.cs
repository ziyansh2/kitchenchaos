using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI moveUpText;
	[SerializeField] private TextMeshProUGUI moveDownText;
	[SerializeField] private TextMeshProUGUI moveLeftText;
	[SerializeField] private TextMeshProUGUI moveRightText;
	[SerializeField] private TextMeshProUGUI interactText;
	[SerializeField] private TextMeshProUGUI interactAlternateText;
	[SerializeField] private TextMeshProUGUI pauseText;

	[SerializeField] private TextMeshProUGUI gamepad_interactText;
	[SerializeField] private TextMeshProUGUI gamepad_interactAlternateText;
	[SerializeField] private TextMeshProUGUI gamepad_pauseText;


	private void Start() {
		GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
        KitchenGameManager.Instance.OnLocalPlayerReadyChange += KitchenGameManager_OnLocalPlayerReadyChange;

		UpdateVisual();
		Show();
	}

    private void KitchenGameManager_OnLocalPlayerReadyChange(object sender, System.EventArgs e) {
		if (KitchenGameManager.Instance.IsLocalPlayerReady()) {
			Hide();
		}
	}

	private void GameInput_OnBindingRebind(object sender, System.EventArgs e) {
		UpdateVisual();
	}

	private void UpdateVisual() {
		moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
		moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
		moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
		moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
		interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
		interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
		pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

		gamepad_interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
		gamepad_interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternate);
		gamepad_pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
	}

	private void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}
}
