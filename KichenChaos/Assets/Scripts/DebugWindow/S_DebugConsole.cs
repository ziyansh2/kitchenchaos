using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_DebugConsole : MonoBehaviour {

	public static S_DebugConsole Instance;

	[SerializeField] private RectTransform displayRect;
	[SerializeField] private TMPro.TextMeshProUGUI displayText;

	float initHeight;

	private void Awake() {
		if (Instance != null) {
			DestroyImmediate(gameObject);
		} else {
			Instance = this;
		}

		initHeight = displayRect.anchoredPosition.y;
	}

	public void ChangeDisplayPosition(float newPosition) {
		displayRect.anchoredPosition = new Vector2(displayRect.anchoredPosition.x, initHeight + newPosition);
	}
}
