using Michsky.UI.Heat;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using UnityEngine;

public class StallManager : MonoBehaviour
{
    public static StallManager Instance;

    [SerializeField] private InputManager inputManager;

    [SerializeField] public GameObject tentPrefab;
    [SerializeField] private GameObject tentPreviewPrefab;
    [SerializeField] private KeyCode hotKey = KeyCode.E;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 1f;

    [SerializeField] private NotificationManager notificationManager;

    [field: SerializeField] public StallDetailsManager StallDetailsManager { get; private set; }

    private GameObject currentPlaceableTent;
    private float mouseWheelRotation = 0;

    [HideInInspector] public PlayerManager playerManager;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inputManager.OnStallToggle += HandleNewObjectHotkey;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (currentPlaceableTent != null)
        {
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();

            ApplicationController.Instance.localPlayer.playerState = PlayerState.IsPlacingTent;
        }
    }

    private void HandleNewObjectHotkey()
    {
        if(ApplicationController.Instance.localPlayer.playerState != PlayerState.IsFree) { return; } 

        if (!StallDetailsManager.isGettingDetails)
        {
            if (currentPlaceableTent != null)
            {
                Destroy(currentPlaceableTent);
            }
            else
            {
                currentPlaceableTent = Instantiate(tentPreviewPrefab);
            }
        }
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
        {
            currentPlaceableTent.transform.position = hitInfo.point;
        }
    }

    private void RotateFromMouseWheel()
    {
        mouseWheelRotation = Input.mouseScrollDelta.y;
        currentPlaceableTent.transform.Rotate(Vector3.up, mouseWheelRotation * rotationSpeed);
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 xzPosition;
            xzPosition.x = currentPlaceableTent.transform.position.x;
            xzPosition.y = currentPlaceableTent.transform.position.z;

            float yRotation = currentPlaceableTent.transform.rotation.eulerAngles.y; ////////////////////////////////////////////////////////////////////////////////////////

            //  Do Math (Take title from UI)
            string title = "Blah Blah";
            string description = "description";

            StallData stallData = new StallData
            {
                xzPosition = xzPosition,
                yRotation = yRotation,
                title = title,
                description = description
            };

            if (currentPlaceableTent.GetComponent<PreviewStall>().IsColliding)
            {
                MainCanvasManager.Instance.notificationManager.notificationText = "Board cannot be placed here, Try Again.";

                MainCanvasManager.Instance.notificationManager.ExpandNotification();
                return;
            }

            playerManager.playerStallPlacement.StallPlacementVerificationServerRpc(stallData);

            Destroy(currentPlaceableTent);
            ResetVariables();
        }
    }

    private void ResetVariables()
    {
        currentPlaceableTent = null;
        mouseWheelRotation = 0;
        ApplicationController.Instance.localPlayer.playerState = PlayerState.IsFree;
    }
}
