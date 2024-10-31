using UnityEngine;

public class S_TriggerColor : MonoBehaviour {

    [SerializeField] protected bool isTriggered = true;

    protected Color colorON;
    protected Color colorOFF;

    protected virtual void Awake() {
        colorOFF = colorON * .3f;
        colorOFF.a = 1f;
        UpdateColor();
    }

    public virtual void TriggerColor() {
        isTriggered = !isTriggered;
        UpdateColor();
    }

    protected virtual void UpdateColor() { }

}
