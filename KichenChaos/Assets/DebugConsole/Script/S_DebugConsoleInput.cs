using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class S_DebugConsoleInput : MonoBehaviour {

	public static S_DebugConsoleInput Instance;

	private TMPro.TMP_InputField debugConsoleInput;

	private Dictionary<string, ActionInfoData> consoleActionMap = new();


	private void Awake() {
		if (Instance != null) {
			DestroyImmediate(gameObject);
		} else {
			Instance = this;
		}

		debugConsoleInput = GetComponent<TMPro.TMP_InputField>();
		debugConsoleInput.onSubmit.AddListener((string key) => {
			string[] inputArray = key.Split(' ');
			string command = inputArray[0];
			object[] inputParams = new object[inputArray.Length - 1];

			for (int i = 0; i < inputParams.Length; i++) {
				inputParams[i] = inputArray[i + 1];
			}

			if (consoleActionMap.ContainsKey(command.ToLower())) {
				MethodInfo methodInfo = consoleActionMap[command.ToLower()].methodInfo;
				var actionOwner = consoleActionMap[command.ToLower()].actionOwner;


				ParameterInfo[] pars = methodInfo.GetParameters();

				for (int i = 0; i < pars.Length; i++){
					Debug.Log(pars[i].ParameterType);
					switch (pars[i].ParameterType.ToString()) {
						case "System.String": break;
						case "System.Int32":
							inputParams[i] =  (inputParams[i].ToString());
							break;
					}
				}

				

				foreach (ParameterInfo p in pars) {
					Debug.Log(p.ParameterType);
				}

				methodInfo.Invoke(actionOwner, inputParams);
			} else {
				Debug.LogWarning($"Console action '{debugConsoleInput.text}' does not exist!");
			}
			debugConsoleInput.text = "";
			debugConsoleInput.ActivateInputField();
		});

		RegisterConsoleAction("help", new ActionInfoData {
			actionOwner = this,
			methodInfo = GetPrivateMethodInfo(GetType(), "PrintAllConsoleAction")
		});
	}

	private void Start() {
		S_DebugConsole.Instance.OnConsoleWindowTriggered += OnConsoleWindowTriggered;

		RegisterConsoleAction("clear", new ActionInfoData {
			actionOwner = S_DebugConsole.Instance,
			methodInfo = GetPublicMethodInfo(typeof(S_DebugConsole), "ClearLog")
		});
	}

	private void OnConsoleWindowTriggered(object sender, bool e) {
		if (e) {
			debugConsoleInput.ActivateInputField();
		} else {
			debugConsoleInput.DeactivateInputField();
		}
	}

	private void OnDestroy() {
		debugConsoleInput.onSubmit.RemoveAllListeners();
	}


	private void PrintAllConsoleAction() {
		string result = "---------Print Out All Console Actions----------";
		int count = 0;
		foreach (var actionName in consoleActionMap.Keys) {
			count++;
			result += $"\n{count}: {actionName}";
		}
		result += "\n---------------------------------------------";
		Debug.Log(result);
	}

	public void RegisterConsoleAction(string key, ActionInfoData actionInfoData) {
		if (consoleActionMap.ContainsKey(key.ToLower())) {
			consoleActionMap[key.ToLower()] = actionInfoData;
		} else {
			consoleActionMap.Add(key.ToLower(), actionInfoData);
		}
	}

	public static MethodInfo GetPrivateMethodInfo(Type methodType, string methodName) {
		return methodType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
	}

	public static MethodInfo GetPublicMethodInfo(Type methodType, string methodName, bool isPublic = true) {
		return methodType.GetMethod(methodName);
	}


	public class ActionInfoData {
		public Component actionOwner;
		public MethodInfo methodInfo;
	}

}
