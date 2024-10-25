using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class S_DebugConsoleInput : MonoBehaviour {

	public static S_DebugConsoleInput Instance;

	private TMP_InputField debugConsoleInput;

	Dictionary<string, Action> consoleActionMap = new();


	private void Awake() {
		if (Instance != null) {
			DestroyImmediate(gameObject);
		} else {
			Instance = this;
		}

		debugConsoleInput = GetComponent<TMP_InputField>();
		debugConsoleInput.onSubmit.AddListener((string key) => {
			if (consoleActionMap.ContainsKey(key.ToLower())) {
				consoleActionMap[key.ToLower()]?.Invoke();
				debugConsoleInput.text = "";
			} else {
				Debug.LogWarning("Console action does not exist!");
			}
		});

		RegisterConsoleAction("help", PrintAllConsoleAction);
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

	public void RegisterConsoleAction(string key, Action action) {
		if (consoleActionMap.ContainsKey(key.ToLower())) {
			consoleActionMap[key.ToLower()] = action;
		} else {
			consoleActionMap.Add(key.ToLower(), action);
		}
	}


}
