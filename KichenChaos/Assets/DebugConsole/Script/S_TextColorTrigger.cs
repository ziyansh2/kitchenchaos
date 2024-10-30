using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class S_TextColorTrigger : S_TriggerColor {

    private TextMeshProUGUI text;

    protected override void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        colorON = text.color;
        base.Awake();
    }

    protected override void UpdateColor() {
        text.color = isTriggered ? colorON : colorOFF;
    }

}
