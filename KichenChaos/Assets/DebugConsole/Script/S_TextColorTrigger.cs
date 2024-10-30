using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class S_TextColorTrigger : MonoBehaviour {

    [SerializeField] private bool isTriggered = true;

    private TextMeshProUGUI text;
    private Color colorON;
    private Color colorOFF;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        colorON = text.color;
        colorOFF = colorON * .5f;
        colorOFF.a = 1f;
        UpdateTextColor();
    }

    public void TriggerColor() {
        isTriggered = !isTriggered;
        UpdateTextColor();
    }

    private void UpdateTextColor() {
        text.color = isTriggered ? colorON : colorOFF;
    }

}
