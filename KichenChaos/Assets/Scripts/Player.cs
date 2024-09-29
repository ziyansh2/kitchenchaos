using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking;

    private void Update() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;

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
                    moveDir = Vector3.zero;
                }
            }
        }

        transform.position += moveDir * moveDistance;

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

        Debug.Log(inputVector);
    }

    private bool IsCanMove(Vector3 moveDir, float moveDistance) {
        float playerRadius = .7f;
        float playerHeight = 2f;
        return !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
    }
    public bool IsWalking() {
        return isWalking;
    }
}
