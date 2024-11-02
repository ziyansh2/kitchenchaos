using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DebugConsole {

	public class S_DebugConsoleContentElement : MonoBehaviour {

		[SerializeField] TMPro.TMP_InputField debugText;
		[SerializeField] Button detailButton;

		private string stackTrace;

		public LogType Type { get; private set; }

		private void Awake() {
			detailButton.onClick.AddListener(() => {
				S_DebugConsole.Instance.ShowStackTrace(stackTrace, Type);
			});
		}
		private void OnDestroy() {
			detailButton.onClick.RemoveAllListeners();
		}

		public void SetDebugContents(string logString, string stackTrace, LogType type) {
			this.stackTrace = stackTrace;
			Type = type;

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

}