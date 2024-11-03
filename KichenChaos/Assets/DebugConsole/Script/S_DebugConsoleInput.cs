using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace DebugConsole {

    public class S_DebugConsoleInput : MonoBehaviour {

        public static S_DebugConsoleInput Instance;

        private TMPro.TMP_InputField debugConsoleInput;

        private Dictionary<string, ActionInfoData> consoleActionMap = new();

        private List<string> commandRecord = new();
        private int commandRecordIndex;


        public EventHandler<ActionRegisterAgs> OnActionRegister;
        public class ActionRegisterAgs : EventArgs {
            public Dictionary<string, ActionInfoData> consoleActionMap;
        }

        private void Awake() {
            if (Instance != null) {
                DestroyImmediate(gameObject);
            } else {
                Instance = this;

                debugConsoleInput = GetComponent<TMPro.TMP_InputField>();
                debugConsoleInput.onSubmit.AddListener((string inputContent) => {
                    string[] inputArray = inputContent.Split(' ');
                    string command = inputArray[0];

                    if (consoleActionMap.ContainsKey(command.ToLower())) {
                        MethodInfo methodInfo = consoleActionMap[command.ToLower()].methodInfo;
                        var actionOwner = consoleActionMap[command.ToLower()].actionOwner;
                        object[] inputParams = CastInputParameters(inputArray, methodInfo);
                        methodInfo.Invoke(actionOwner, inputParams);
                        commandRecord.Add(inputContent);
                    } else {
                        Debug.LogWarning($"Console action '{debugConsoleInput.text}' does not exist!");
                    }
                    debugConsoleInput.text = "";
                    FocusInputField();
                });
            }
        }

        private void Start() {
            S_DebugConsole.Instance.OnConsoleWindowTriggered += OnConsoleWindowTriggered;
            RegisterConsoleAction("clear", MakeActionInfoData(S_DebugConsole.Instance, "ClearLog", "Clear all logs."));
        }

        private void OnDestroy() {
            debugConsoleInput?.onSubmit.RemoveAllListeners();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                commandRecordIndex = Math.Max(commandRecordIndex - 1, 0);

                if (debugConsoleInput.isFocused && commandRecord.Count > 0) {
                    debugConsoleInput.text = commandRecord[commandRecordIndex];
                }
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                commandRecordIndex = Math.Min(commandRecordIndex + 1, commandRecord.Count - 1);
                
                if (debugConsoleInput.isFocused && commandRecord.Count > 0) {
                    debugConsoleInput.text = commandRecord[commandRecordIndex];
                }
            }
        }

        private object[] CastInputParameters(string[] inputArray, MethodInfo methodInfo) {
            object[] inputParams = new object[inputArray.Length - 1];
            Array.Copy(inputArray, 1, inputParams, 0, inputParams.Length);

            ParameterInfo[] pars = methodInfo.GetParameters();
            for (int i = 0; i < pars.Length; i++) {
                //Debug.Log(pars[i].ParameterType);
                switch (pars[i].ParameterType.ToString()) {
                    case "System.String":   //string
                        break;
                    case "System.Int32":    //int
                        inputParams[i] = int.Parse(inputParams[i].ToString());
                        break;
                    case "System.Single":   //float
                        inputParams[i] = float.Parse(inputParams[i].ToString());
                        break;
                    case "System.Boolean":  //bool
                        inputParams[i] = bool.Parse(inputParams[i].ToString());
                        break;
                }
            }
            return inputParams;
        }

        private void OnConsoleWindowTriggered(object sender, bool e) {
            if (e) {
                FocusInputField();
            } else {
                debugConsoleInput.DeactivateInputField();
            }
        }

        public void FocusInputField() {
            debugConsoleInput.ActivateInputField();
        }


        public static void RegisterConsoleAction(string key, ActionInfoData actionInfoData) {
            Instance.RegisterConsoleActionInternal(key, actionInfoData);
        }

        public static ActionInfoData MakeActionInfoData(Component component, string methodName, string methodDescription = "", bool isPublic = true) {
            ActionInfoData actionInfoData = null;
            if (isPublic) {
                actionInfoData = new ActionInfoData {
                    actionOwner = component,
                    methodInfo = GetPublicMethodInfo(component.GetType(), methodName),
                    methodName = methodName,
                    methodDescription = methodDescription
                };
            } else {
                actionInfoData = new ActionInfoData {
                    actionOwner = component,
                    methodInfo = GetPrivateMethodInfo(component.GetType(), methodName),
                    methodName = methodName,
                    methodDescription = methodDescription
                };
            }
            return actionInfoData;
        }

        private void RegisterConsoleActionInternal(string key, ActionInfoData actionInfoData) {
            if (consoleActionMap.ContainsKey(key.ToLower())) {
                consoleActionMap[key.ToLower()] = actionInfoData;
            } else {
                consoleActionMap.Add(key.ToLower(), actionInfoData);
            }
            OnActionRegister?.Invoke(this, new ActionRegisterAgs() { consoleActionMap = consoleActionMap });
        }

        private static MethodInfo GetPrivateMethodInfo(Type methodType, string methodName) {
            return methodType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static MethodInfo GetPublicMethodInfo(Type methodType, string methodName, bool isPublic = true) {
            return methodType.GetMethod(methodName);
        }

        public class ActionInfoData {
            public Component actionOwner;
            public MethodInfo methodInfo;
            public string methodName;
            public string methodDescription;
        }

    }
}