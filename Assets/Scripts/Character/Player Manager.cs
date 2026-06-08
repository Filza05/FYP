using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

//Calling ALL Movement and Rotation Functions and RUNNING them here.
public enum PlayerState
{
    IsFree,
    IsInteracting,
    InChatMenu,
    IsPlacingTent
}
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private InputManager inputManager;

    PlayerLocomotion playerLocomotion;

    public PlayerStallPlacement playerStallPlacement;
    public PlayerInfoBroadcast playerInfoBroadcast;

    public PlayerState playerState = PlayerState.IsFree; 

    public override void OnNetworkSpawn()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerStallPlacement = GetComponent<PlayerStallPlacement>();
        playerInfoBroadcast = GetComponent<PlayerInfoBroadcast>();

        if(!IsOwner) { return ; }
        StallManager.Instance.playerManager = this;
        ApplicationController.Instance.localPlayer = this;
    }

    private void Update()
    {
        if(IsOwner)
            inputManager.HandleAllInputs();
    }

    //fixedUpdate works better for rigidbodies.
    private void FixedUpdate()
    {
        if (IsOwner)
            playerLocomotion.HandleAllMovementAndRotation();
    }

    //lateUpdate works better for cameras.
    private void LateUpdate()
    {
        if (IsOwner && MainCameraManager.Instance != null)
            MainCameraManager.Instance.HandleAllCameraMovement();
    }
}
