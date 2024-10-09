using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour {

	public static OptionsUI Instance { get; private set; }

	[SerializeField] private Button soundEffectsButton;
	[SerializeField] private Button musicButton;
	[SerializeField] private Button closeButton;

	[SerializeField] private Button moveUpButton;
	[SerializeField] private Button moveDownButton;
	[SerializeField] private Button moveLeftButton;
	[SerializeField] private Button moveRightButton;
	[SerializeField] private Button interactButton;
	[SerializeField] private Button interactAlternateButton;
	[SerializeField] private Button pauseButton;

	[SerializeField] private Button gamepad_interactButton;
	[SerializeField] private Button gamepad_interactAlternateButton;
	[SerializeField] private Button gamepad_pauseButton;

	[SerializeField] private TextMeshProUGUI soundEffectsText;
	[SerializeField] private TextMeshProUGUI musicText;

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

	[SerializeField] private Transform PressToRebindKeyTransform;

	private void Awake() {
		Instance = this;

		soundEffectsButton.onClick.AddListener(() => {
			SoundManager.Instance.ChangeVolume();
			UpdateVisual();
		});
		musicButton.onClick.AddListener(() => {
			MusicManager.Instance.ChangeVolume();
			UpdateVisual();
		});

		closeButton.onClick.AddListener(() => {
			Hide();
		});

		moveUpButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Up));
		moveDownButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Down));
		moveLeftButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Left));
		moveRightButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Right));
		interactButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
		interactAlternateButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlternate));
		pauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));

		gamepad_interactButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Interact));
		gamepad_interactAlternateButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_InteractAlternate));
		gamepad_pauseButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Pause));
	}

	private void Start() {
		KitchenGameManager.Instance.OnUnpaused += KitchenGameManager_OnUnpaused;

		UpdateVisual();
		Hide();
		HidePressToRebindKey();
	}

	private void KitchenGameManager_OnUnpaused(object sender, System.EventArgs e) {
		Hide();
	}

	private void UpdateVisual() {
		soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
		musicText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

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

	public void Show() {
		gameObject.SetActive(true);
	}

	private void Hide() {
		gameObject.SetActive(false);
	}

	private void ShowPressToRebindKey() {
		PressToRebindKeyTransform.gameObject.SetActive(true);
	}

	private void HidePressToRebindKey() {
		PressToRebindKeyTransform.gameObject.SetActive(false);
	}

	private void RebindBinding(GameInput.Binding binding) {
		ShowPressToRebindKey();
		GameInput.Instance.RebindBinding(binding, () => { 
			HidePressToRebindKey();
			UpdateVisual();
		});
	}

}
