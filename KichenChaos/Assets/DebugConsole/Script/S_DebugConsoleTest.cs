using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugConsole;

public class S_DebugConsoleTest : MonoBehaviour {

	[SerializeField] private bool isTest;


	private void Start() {
		S_DebugConsoleInput.RegisterConsoleAction("test1", S_DebugConsoleInput.MakeActionInfoData(this, "ConsoleActionTest1"));
		S_DebugConsoleInput.RegisterConsoleAction("test2", S_DebugConsoleInput.MakeActionInfoData(this, "ConsoleActionTest2", "Test Something", false));
	}

	public void ConsoleActionTest1() {
		Debug.Log("Run Console Test Action 1");
	}

	private void ConsoleActionTest2(bool testValue, float testValue2) {
		Debug.Log($"Run Console Test Action2: {testValue}, {testValue2}");
	}

	void Update() {
		UpdateDebugTest();
	}

	private void UpdateDebugTest() {
		if (!isTest) return;

		if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) {
			Debug.LogWarning("Warning!");
		} else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2)) {
			Debug.LogError("Error!!!!!");
		} else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) {
			Debug.Log("Log....");
		}
	}

}
