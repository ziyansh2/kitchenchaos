using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DebugConsole {

    public class S_DebugConsoleHelp : MonoBehaviour {

        [SerializeField] private GameObject debugConsoleHelpElement;
        [SerializeField] private ScrollRect debugConsoleHelpScroll;


        private void Start() {
            S_DebugConsoleInput.Instance.OnActionRegister += DebugConsoleInput_OnActionRegister;
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
    }

}
