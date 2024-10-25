using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class S_DebugConsoleTest : MonoBehaviour {

	[SerializeField] private bool isTest;


	private void Start() {
		S_DebugConsoleInput.Instance.RegisterConsoleAction("test action", ConsoleAction);
	}

	private void ConsoleAction() {
		Debug.Log("Run Console Test Action");
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
