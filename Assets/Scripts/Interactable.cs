using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool playerWasFree = true;

    protected void OnEnable()
    {
        playerWasFree = true;

        if(ApplicationController.Instance.localPlayer == null) { return; }

        if (ApplicationController.Instance.localPlayer.playerState == PlayerState.IsInteracting)
        {
            playerWasFree = false;
            gameObject.SetActive(false);
            return;
        }

        ApplicationController.Instance.localPlayer.playerState = PlayerState.IsInteracting;
        ApplicationController.Instance.inputManager.playerControls.Disable();
    }

    protected void OnDisable()
    {
        if(!playerWasFree)
        {
            return;
        }

        ApplicationController.Instance.localPlayer.playerState = PlayerState.IsFree;
        ApplicationController.Instance.inputManager.playerControls.Enable();
    }
}
