using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_DebugConsole : MonoBehaviour {

	public static S_DebugConsole Instance;

	[SerializeField] private RectTransform displayRect;

	[Space(10)]
	[SerializeField] private GameObject debugConsoleElement;
	[SerializeField] private RectTransform debugContentOwner;

	[Space(10)]
	[SerializeField] private RectTransform stackTraceRect;
	[SerializeField] private TMPro.TextMeshProUGUI stackTraceText;

	[Space(10)]
	[SerializeField] private bool isTest;

	float initHeight;


	private void Awake() {
		if (Instance != null) {
			DestroyImmediate(gameObject);
		} else {
			Instance = this;
		}

		initHeight = displayRect.anchoredPosition.y;
	}

	private void OnEnable() {
		Application.logMessageReceived += HandleLog;
	}

	private void Start() {
		HideStackTrace();
	}

	private void Update() {
		UpdateDebugTest();
	}

	private void OnDisable() {
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string logString, string stackTrace, LogType type) {
		S_DebugConsoleElement element = Instantiate(debugConsoleElement, debugContentOwner).GetComponent<S_DebugConsoleElement>();
		element.SetDebugContents(logString, stackTrace, type);
	}

	private void UpdateDebugTest() {
		if (!isTest) return;

		if (Input.GetKeyDown(KeyCode.W)) {
			Debug.LogWarning("Warning!");
		} else if (Input.GetKeyDown(KeyCode.E)) {
			Debug.LogError("Error!!!!!");
		} else if (Input.GetKeyDown(KeyCode.L)) {
			Debug.Log("Log....");
		}
	}

	public void ChangeDisplayPosition(float newPosition) {
		displayRect.anchoredPosition = new Vector2(displayRect.anchoredPosition.x, initHeight + newPosition);
	}

	public void ShowStackTrace(string stackTrace) {
		stackTraceRect.gameObject.SetActive(true);
		stackTraceText.text = stackTrace;
	}

	public void HideStackTrace() {
		stackTraceRect.gameObject.SetActive(false);
	}
}
