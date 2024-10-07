using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayingClockUI : MonoBehaviour {

	[SerializeField] private Image timerImage;

	void Update() {
		timerImage.fillAmount = KitchenGameManager.Instance.GetGamePlayingTimerNormalized();
	}

}
