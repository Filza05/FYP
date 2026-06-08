using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//handling player movement and rotation logic here.
public class PlayerLocomotion : NetworkBehaviour
{
    [SerializeField] private InputManager inputManager;

    Vector3 movementDirection;
    Rigidbody playerRigidBody;

    public float movementSpeed = 10f;
    public float rotationSpeed = 15f;

    public override void OnNetworkSpawn()
    {
        playerRigidBody = GetComponent<Rigidbody>();   
    }

    private void HandleMovement()
    {
        if(MainCameraManager.Instance == null) { return; }

        movementDirection = MainCameraManager.Instance.transform.forward * inputManager.zAxisInputReceived;
        movementDirection += MainCameraManager.Instance.transform.right * inputManager.xAxisInputReceived;
        movementDirection.Normalize();
        movementDirection.y = 0;
        movementDirection = movementDirection * movementSpeed;
        Vector3 movementVelocity = movementDirection;

        playerRigidBody.velocity = movementVelocity;    
    }

    private void HandleRotation()
    {
        if (MainCameraManager.Instance == null) { return; }
        Vector3 targetDirection = Vector3.zero;

        targetDirection = MainCameraManager.Instance.transform.forward * inputManager.zAxisInputReceived;
        targetDirection += MainCameraManager.Instance.transform.right * inputManager.xAxisInputReceived;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion rotatePlayer = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = rotatePlayer;
    }

    public void HandleAllMovementAndRotation()
    {
        HandleMovement();
        HandleRotation();
    }
}
