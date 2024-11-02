using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static DebugConsole.S_DebugConsoleInput;

namespace DebugConsole {

	public class S_DebugConsoleHelpElement : MonoBehaviour {

		[SerializeField] TMPro.TMP_InputField commandText;
		[SerializeField] TMPro.TextMeshProUGUI parametersText;
		[SerializeField] TMPro.TextMeshProUGUI descriptionText;

		static readonly Dictionary<string, string> parameterTypeMap = new () {
			{ "System.String", "string" },
			{ "System.Int32", "int" },
			{ "System.Single", "float" },
			{ "System.Boolean", "bool" }
		};

		public void SetMethodContents(string command, ActionInfoData actionInfoData) {
			commandText.text = command;
			descriptionText.text = actionInfoData.methodDescription;

			ParameterInfo[] pars = actionInfoData.methodInfo.GetParameters();
			string paramaterView = "";
            for (int i = 0; i < pars.Length; i++) {
				string typeName = pars[i].ParameterType.ToString();
				if (parameterTypeMap.ContainsKey(typeName)){
					typeName = parameterTypeMap[typeName];
				}
				paramaterView += $"{pars[i].Name} ({typeName})";
				if (i != pars.Length - 1) {
					paramaterView += "\n";
				}
			}
            parametersText.text = paramaterView;
		}
	}

}