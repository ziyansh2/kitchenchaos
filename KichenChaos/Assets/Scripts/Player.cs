using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        float interactDistance = 2f;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                clearCounter.Interact();
            }
        } 
    }

    private void Update() {
        HandleMovement();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        if (!IsCanMove(moveDir, moveDistance)) {
            //Cannot move towards moveDir
            //Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            if (IsCanMove(moveDirX, moveDistance)) {
                //Can move only on the X
                moveDir = moveDirX;
            } else {
                //Cannot move towards moveDir
                //Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                if (IsCanMove(moveDirZ, moveDistance)) {
                    //Can move only on the Z
                    moveDir = moveDirZ;
                } else {
                    //Can not move at all
                    moveDir = Vector3.zero;
                }
            }
        }

        isWalking = moveDir != Vector3.zero;
        if (isWalking) {
            transform.position += moveDir * moveDistance;
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }
    }

    private bool IsCanMove(Vector3 moveDir, float moveDistance) {
        float playerRadius = .7f;
        float playerHeight = 2f;
        return !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
    }
}
