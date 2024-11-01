using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class S_DebugConsole : MonoBehaviour {

	public static S_DebugConsole Instance;

	public EventHandler<bool> OnConsoleWindowTriggered;

	[Header("Visual")]
	[SerializeField] private RectTransform displayRect;
	[SerializeField] private Slider consoleSlider;

	[Space(10), Header("Log Elements")]
	[SerializeField] private GameObject debugConsoleElement;
	[SerializeField] private RectTransform debugContentOwner;

	[Space(10), Header("Stack Trace")]
	[SerializeField] private RectTransform stackTraceRect;
	[SerializeField] private TMPro.TextMeshProUGUI stackTraceTitleText;
	[SerializeField] private TMPro.TMP_InputField stackTraceText;

	[Space(10), Header("Input")]
	[SerializeField] private KeyCode debugConsoleTrigger = KeyCode.F12;

	//Visual
	private float initHeight;
	private float windowPosition;

	//Filters
	private bool isLogTriggered = true;
	private bool isWarningTriggered = true;
	private bool isErrorTriggered = true;
	private bool isCollapseTriggered = false;

	//Elements
	private List<S_DebugConsoleElement> consoleElements = new();
	private Dictionary<string, S_DebugConsoleElement> collapsedElements = new();


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

	private void OnDisable() {
		Application.logMessageReceived -= HandleLog;
	}

	private void Start() {
		consoleSlider.onValueChanged.AddListener(ChangeDisplayPosition);
		HideStackTrace();
		consoleSlider.value = consoleSlider.minValue;
	}

	private void Update() {
		if (Input.GetKeyDown(debugConsoleTrigger)) {
			if (windowPosition > 0) {
				consoleSlider.value = consoleSlider.minValue;
			} else {
				consoleSlider.value = consoleSlider.maxValue;
			}
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type) {
		S_DebugConsoleElement element = Instantiate(debugConsoleElement, debugContentOwner).GetComponent<S_DebugConsoleElement>();
		element.SetDebugContents(logString, stackTrace, type);
		consoleElements.Add(element);
		if (collapsedElements.Keys.Contains(logString)) {
			collapsedElements[logString]= element;
		} else {
			collapsedElements.Add(logString, element);
		}
		
		UpdateCollapseFilter();
	}

	private void ChangeDisplayPosition(float newPosition) {
		windowPosition = newPosition;
		displayRect.anchoredPosition = new Vector2(displayRect.anchoredPosition.x, initHeight + newPosition);
		OnConsoleWindowTriggered?.Invoke(this, newPosition > 0);
	}

	public void ShowStackTrace(string stackTrace, LogType type) {
		stackTraceRect.gameObject.SetActive(true);
		stackTraceText.text = stackTrace;

		Color color = Color.white;
		if (type == LogType.Warning) {
			color = (Color.yellow + Color.red) / 2;
		} else if (type == LogType.Error) { 
			color = Color.red;
		}
		stackTraceTitleText.color = color;
		stackTraceTitleText.text = $"Detail Window ({type.ToString()})"; 
	}

	public void HideStackTrace() {
		stackTraceRect.gameObject.SetActive(false);
	}

	public void ClearLog() {
		while (debugContentOwner.childCount > 0) {
			DestroyImmediate(debugContentOwner.GetChild(0).gameObject);
		}
		consoleElements.Clear();
		collapsedElements.Clear();
	}


	#region Console Log Filter

	public void TriggerLogFiltter() {
		isLogTriggered = !isLogTriggered;
		UpdateAfterFilterUpdate();
	}

	public void TriggerWarningFiltter() {
		isWarningTriggered = !isWarningTriggered;
		UpdateAfterFilterUpdate();
	}

	public void TriggerErrorFiltter() {
		isErrorTriggered = !isErrorTriggered;
		UpdateAfterFilterUpdate();
	}

	private void UpdateAfterFilterUpdate() {
		foreach (S_DebugConsoleElement consoleElement in consoleElements) {
			switch (consoleElement.Type) {
				default:
				case LogType.Log:
					consoleElement.gameObject.SetActive(isLogTriggered);
					break;
				case LogType.Warning:
					consoleElement.gameObject.SetActive(isWarningTriggered);
					break;
				case LogType.Error:
					consoleElement.gameObject.SetActive(isErrorTriggered);
					break;
			}
		}
	}

	public void TriggerCollapseFilter() {
		isCollapseTriggered = !isCollapseTriggered;
		UpdateCollapseFilter();
	}

	private void UpdateCollapseFilter() {
		foreach (S_DebugConsoleElement consoleElement in consoleElements) {
			bool isVisible = !isCollapseTriggered || (isCollapseTriggered && collapsedElements.Values.Contains(consoleElement));
			consoleElement.gameObject.SetActive(isVisible);
		}
	}

	#endregion

}
