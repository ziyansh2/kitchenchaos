using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DebugConsole {

    public class S_DebugConsoleHelp : MonoBehaviour {

        [SerializeField] private GameObject debugConsoleHelpElement;
        [SerializeField] private ScrollRect debugConsoleHelpScroll;
        [SerializeField] private GameObject debugConsoleHelpWindow;


        private void Start() {
            //The help window rely on the debug console and debug console input
            if (S_DebugConsole.Instance == null || S_DebugConsoleInput.Instance == null) {
                debugConsoleHelpWindow.SetActive(false);
                return;
            }

            S_DebugConsole.Instance.OnConsoleWindowTriggered += DebugConsole_OnConsoleWindowTriggered;

            S_DebugConsoleInput.Instance.OnActionRegister += DebugConsoleInput_OnActionRegister;
            S_DebugConsoleInput.RegisterConsoleAction("help", S_DebugConsoleInput.MakeActionInfoData(this, "TriggerHelpWindow", "Trigger the window shows all command information."));
            debugConsoleHelpWindow.SetActive(false);
        }

        private void DebugConsole_OnConsoleWindowTriggered(object sender, bool isTriggered) {
            if (isTriggered == false) {
                debugConsoleHelpWindow.SetActive(false);
            }
        }

        private void DebugConsoleInput_OnActionRegister(object sender, S_DebugConsoleInput.ActionRegisterAgs e) {
            while (debugConsoleHelpScroll.content.childCount > 0) {
                DestroyImmediate(debugConsoleHelpScroll.content.GetChild(0).gameObject);
            }

            foreach (var element in e.consoleActionMap) {
                S_DebugConsoleHelpElement helpElement = Instantiate(debugConsoleHelpElement, debugConsoleHelpScroll.content).GetComponent<S_DebugConsoleHelpElement>();
                helpElement.SetMethodContents(element.Key, element.Value);
            }
        }

        public void TriggerHelpWindow() {
            debugConsoleHelpWindow.SetActive(debugConsoleHelpWindow.activeSelf ? false : true);
            S_DebugConsoleInput.Instance?.FocusInputField();
        }

    }

}
