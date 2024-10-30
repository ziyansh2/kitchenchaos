using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class S_OutlineColorTrigger : S_TriggerColor {

    private Outline outline;

    protected override void Awake() {
        outline = GetComponent<Outline>();
        colorON = outline.effectColor;
        base.Awake();
    }

    protected override void UpdateColor() {
        outline.effectColor = isTriggered ? colorON : colorOFF;
    }

}
