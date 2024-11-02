using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DebugConsole.S_DebugConsoleInput;

namespace DebugConsole {

	public class S_DebugConsoleHelpElement : MonoBehaviour {

		[SerializeField] TMPro.TMP_InputField commandText;
		[SerializeField] TMPro.TextMeshProUGUI parametersText;
		[SerializeField] TMPro.TextMeshProUGUI descriptionText;



		public void SetMethodContents(string command, ActionInfoData actionInfoData) {
			commandText.text = command;
			descriptionText.text = actionInfoData.methodDescription;
			parametersText.text = "";
		}
	}

}