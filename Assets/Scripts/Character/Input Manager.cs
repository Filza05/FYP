using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Reading Button input values from the player.

[CreateAssetMenu(fileName = "New Input Manager", menuName = "input/input manager")]
public class InputManager : ScriptableObject
{
    public PlayerControls playerControls;

    Vector2 movementInputReceived;
    public float zAxisInputReceived;
    public float xAxisInputReceived;
    private float moveAmount;

    Vector2 cameraInputReceived;
    public float cameraInputX;
    public float cameraInputY;

    public event Action<float> OnMove;
    public event Action OnStallToggle;

    private void OnEnable()
    {
        if (playerControls == null) {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInputReceived = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInputReceived = i.ReadValue<Vector2>();
            playerControls.Interaction.StallToggle.performed += HandleStallToggle;

            playerControls.Enable();
        }
    }

    private void OnDisable()
    {
        playerControls.PlayerMovement.Movement.performed -= i => movementInputReceived = i.ReadValue<Vector2>();
        playerControls.PlayerMovement.Camera.performed -= i => cameraInputReceived = i.ReadValue<Vector2>();
        playerControls.Interaction.StallToggle.performed -= HandleStallToggle;

        playerControls.Disable();   
    }

    public void HandleAllInputs()
    {
        //All character related input functions called here
        HandleMovementInput();
        HandleCameraInput();
    }

    private void HandleMovementInput()
    {
        zAxisInputReceived = movementInputReceived.y;
        xAxisInputReceived = movementInputReceived.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(zAxisInputReceived) + Mathf.Abs(xAxisInputReceived));
        OnMove.Invoke(moveAmount);
        /*animatorManager.UpdateAnimatorValues(0, moveAmount);*/
    }

    private void HandleCameraInput()
    {
        cameraInputX = cameraInputReceived.x;
        cameraInputY = cameraInputReceived.y;
    }

    private void HandleStallToggle(InputAction.CallbackContext context)
    {
        if(SceneManager.GetActiveScene().name == "Game")
        OnStallToggle?.Invoke();
    }
}
