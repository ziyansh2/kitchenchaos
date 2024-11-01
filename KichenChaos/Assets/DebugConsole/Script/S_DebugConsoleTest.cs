using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class S_DebugConsoleTest : MonoBehaviour {

	[SerializeField] private bool isTest;


	private void Start() {
		S_DebugConsoleInput.Instance.RegisterConsoleAction("test1",
			new S_DebugConsoleInput.ActionInfoData {
				actionOwner = this,
				methodInfo = S_DebugConsoleInput.GetPublicMethodInfo(GetType(), "ConsoleActionTest1")
			});
		
		S_DebugConsoleInput.Instance.RegisterConsoleAction("test2", 
			new S_DebugConsoleInput.ActionInfoData {
				actionOwner = this,
				methodInfo = S_DebugConsoleInput.GetPrivateMethodInfo(GetType(), "ConsoleActionTest2")
			});
	}

	public void ConsoleActionTest1() {
		Debug.Log("Run Console Test Action 1");
	}

	private void ConsoleActionTest2(int testIndex) {
		Debug.Log($"Run Console Test Action2: {testIndex}");
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
