using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance { get; private set; }

	[SerializeField] AudioClipRefsSO audioClipRefsSO;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
		DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
		CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
		Player.Instance.OnPickedSomething += Instance_OnPickedSomething;
		BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
		TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
	}

	private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
		PlaySound(audioClipRefsSO.trash, (sender as MonoBehaviour).transform.position);
	}

	private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e) {
		PlaySound(audioClipRefsSO.objectDrop, (sender as MonoBehaviour).transform.position);
	}

	private void Instance_OnPickedSomething(object sender, System.EventArgs e) {
		PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
	}

	private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) {
		PlaySound(audioClipRefsSO.chop, (sender as MonoBehaviour).transform.position);
	}

	private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
		PlaySound(audioClipRefsSO.deliveryFail, DeliveryCounter.Instance.transform.position);
	}

	private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
		PlaySound(audioClipRefsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
	}

	private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
		AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)] , position, volume);
	}

	private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
		AudioSource.PlayClipAtPoint(audioClip, position, volume);
	}

	public void PlayFootstepsSound(Vector3 position, float volume = 1) {
		PlaySound(audioClipRefsSO.footstep, position, volume);
	}
}