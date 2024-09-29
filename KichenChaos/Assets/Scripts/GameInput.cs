using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {

    private PlayerInputAction playerInputAction;

    public event EventHandler OnInteractAction;

    private void Awake() {
        playerInputAction = new PlayerInputAction();
        playerInputAction.Player.Enable();

        playerInputAction.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }
}
