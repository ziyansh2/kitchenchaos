using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Windows;

public class S_DebugConsoleInput : MonoBehaviour {

	public static S_DebugConsoleInput Instance;

	private TMP_InputField debugConsoleInput;

	private Dictionary<string, Action> consoleActionMap = new();


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
			} else {
				Debug.LogWarning($"Console action '{debugConsoleInput.text}' does not exist!");
			}
			debugConsoleInput.text = "";
			debugConsoleInput.ActivateInputField();
		});

		RegisterConsoleAction("help", PrintAllConsoleAction);
	}

	private void Start() {
		S_DebugConsole.Instance.OnConsoleWindowTriggered += OnConsoleWindowTriggered;
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

	public void RegisterConsoleAction(string key, Action action) {
		if (consoleActionMap.ContainsKey(key.ToLower())) {
			consoleActionMap[key.ToLower()] = action;
		} else {
			consoleActionMap.Add(key.ToLower(), action);
		}
	}

}
