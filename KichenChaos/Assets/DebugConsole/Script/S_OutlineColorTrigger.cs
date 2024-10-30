using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class S_OutlineColorTrigger : MonoBehaviour {

    [SerializeField] private bool isTriggered = true;

    private Outline outline;
    private Color colorON;
    private Color colorOFF;

    private void Awake() {
        outline = GetComponent<Outline>();
        colorON = outline.effectColor;
        colorOFF = colorON * .5f;
        colorOFF.a = 1f;
        UpdateTextColor();
    }

    public void TriggerColor() {
        isTriggered = !isTriggered;
        UpdateTextColor();
    }

    private void UpdateTextColor() {
        outline.effectColor = isTriggered ? colorON : colorOFF;
    }

}
