using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_DebugConsoleElement : MonoBehaviour {

	[SerializeField] TMPro.TextMeshProUGUI debugText;
	[SerializeField] Button detailButton;

	private string stackTrace;

	private void Awake() {
		detailButton.onClick.AddListener(() => {
			S_DebugConsole.Instance.ShowStackTrace(stackTrace);
		});
	}
	private void OnDestroy() {
		detailButton.onClick.RemoveAllListeners();
	}

	public void SetDebugContents(string logString, string stackTrace, LogType type) {
		this.stackTrace = stackTrace;

		switch (type) {
			default:
			case LogType.Log:
				debugText.text = logString;
				break;
			case LogType.Warning:
				debugText.text = $"<color=orange>" + logString + "<color=orange>\n";
				break;
			case LogType.Error:
				debugText.text = $"<color=red>" + logString + "<color=red>\n";
				break;
		}
	}

}
