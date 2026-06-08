using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimatorManager : NetworkBehaviour
{
    [SerializeField] InputManager inputManager;

    Animator animator;
    int horizontalValue;
    int verticalValue;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontalValue = Animator.StringToHash("Horizontal");
        verticalValue = Animator.StringToHash("Vertical");
    }

    private void Start()
    {
        // Only subscribe to input events if this is the local player (owner)
        if (IsOwner)
        {
            inputManager.OnMove += HandleOnMove;
        }
    }

    //Otherwise Error shows up that Animator component has been destroyed...
    private void OnDestroy()
    {
        if (IsOwner)
        {
            inputManager.OnMove -= HandleOnMove;
        }
    }


    private void HandleOnMove(float moveAmount)
    {
        if (IsOwner)
        {
            // Update animator locally only if this client owns the object, otherwise animations displayed for all clients if local client moves.
            UpdateAnimatorValues(0, moveAmount);
        }
    }

    public void UpdateAnimatorValues(float horizontalInput, float verticalInput)
    {
        //Animation Snapping
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal Animation
        if (horizontalInput > 0 && horizontalInput < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalInput > 0.55f)
        {
            snappedHorizontal = 1.0f;
        }
        else if (horizontalInput < 0.0f && horizontalInput > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalInput < -0.55f)
        {
            snappedHorizontal = -1.0f;
        }
        else
        {
            snappedHorizontal = 0.0f;
        }
        #endregion

        #region Snapped Vertical Animation
        if (verticalInput > 0 && verticalInput < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalInput > 0.55f)
        {
            snappedVertical = 1.0f;
        }
        else if (verticalInput < 0.0f && verticalInput > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalInput < -0.55f)
        {
            snappedVertical = -1.0f;
        }
        else
        {
            snappedVertical = 0.0f;
        }
        #endregion

        //if (animator == null) animator = GetComponent<Animator>();
        animator.SetFloat(horizontalValue, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalValue, snappedVertical, 0.1f, Time.deltaTime);
    }
}
