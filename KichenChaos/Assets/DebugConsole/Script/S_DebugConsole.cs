using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace DebugConsole {

	public class S_DebugConsole : MonoBehaviour {

		public static S_DebugConsole Instance;

		public EventHandler<bool> OnConsoleWindowTriggered;

		[Header("Visual")]
		[SerializeField] private RectTransform displayRect;
		[SerializeField] private Slider consoleSlider;

		[Space(10), Header("Log Elements")]
		[SerializeField] private GameObject debugConsoleElement;
		[SerializeField] private ScrollRect debugContentScrollRect;

		[Space(10), Header("Stack Trace")]
		[SerializeField] private RectTransform stackTraceRect;
		[SerializeField] private TMPro.TextMeshProUGUI stackTraceTitleText;
		[SerializeField] private TMPro.TMP_InputField stackTraceText;

		[Space(10), Header("Input")]
		[SerializeField] private KeyCode debugConsoleTrigger = KeyCode.F12;

		[Space(10), Header("Others")]
		[SerializeField] private bool isAvailableOnShippingBuild = false;

		//Visual
		private float initHeight;
		private float windowPosition;

		//Filters
		private bool isLogTriggered = true;
		private bool isWarningTriggered = true;
		private bool isErrorTriggered = true;
		private bool isCollapseTriggered = false;

		//Elements
		private List<S_DebugConsoleContentElement> consoleElements = new();
		private Dictionary<string, S_DebugConsoleContentElement> collapsedElements = new();
		private RectTransform debugContentsRoot;


		private void Awake() {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			if (Instance != null) {
				DestroyImmediate(gameObject);	
			} else {
				Instance = this;
				DontDestroyOnLoad(gameObject);

				initHeight = displayRect.anchoredPosition.y;
				debugContentsRoot = debugContentScrollRect.GetComponent<RectTransform>();
			}
#else
				if(!isAvailableOnShippingBuild){
					DestroyImmediate(Instance.gameObject);
					return;
				}
#endif
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
			S_DebugConsoleContentElement element = Instantiate(debugConsoleElement, debugContentScrollRect.content).GetComponent<S_DebugConsoleContentElement>();

			element.SetDebugContents(logString, stackTrace, type);
			consoleElements.Add(element);
			if (collapsedElements.Keys.Contains(logString)) {
				collapsedElements[logString] = element;
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
			stackTraceTitleText.text = $"Detail Window ({type})";
		}

		public void HideStackTrace() {
			stackTraceRect.gameObject.SetActive(false);
		}

		public void ClearLog() {
			while (debugContentScrollRect.content.childCount > 0) {
				DestroyImmediate(debugContentScrollRect.content.GetChild(0).gameObject);
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
			foreach (S_DebugConsoleContentElement consoleElement in consoleElements) {
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
			foreach (S_DebugConsoleContentElement consoleElement in consoleElements) {
				bool isVisible = !isCollapseTriggered || (isCollapseTriggered && collapsedElements.Values.Contains(consoleElement));
				consoleElement.gameObject.SetActive(isVisible);
			}

			//Update the content root immediately
			LayoutRebuilder.ForceRebuildLayoutImmediate(debugContentScrollRect.content);
			LayoutRebuilder.ForceRebuildLayoutImmediate(debugContentsRoot);
			Invoke("ScrollToEnd", .05f);
		}

		private void ScrollToEnd() {
			debugContentScrollRect.normalizedPosition = new Vector2(0, 0);  //scroll to the end
		}

		#endregion

	}
}