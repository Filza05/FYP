using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MainCameraManager : MonoBehaviour
{
    public static MainCameraManager Instance {  get; private set; }

    public InputManager inputManager;

    public Transform targetTransform; // player's transform that camera will follow
    public Transform cameraPivot; //the object the camera uses to pivot.
    public Transform cameraTransform; //transform of the actual camera object in the scene.
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public LayerMask collisionLayers; //layers we want our camera to collide with.
    private Vector3 cameraVectorPosition; // to actually manipulate camera position after collision.

    public float cameraFollowSpeed = 0.25f;
    public float cameraLookSpeed = 16;
    public float cameraPivotSpeed = 16;
    public float camLookSmoothTime = 1;

    public float lookAngle;
    public float pivotAngle;
    public float minPivotAngle = -25;
    public float maxPivotAngle = 25;

    private float defaultPosition; //the position camera comes back to after collision has finished. (-3.5z)
    public float cameraCollisionRadius = 0.3f;
    public float cameraCollisionOffset = 0.2f; // how much the camera will jump off the object it collided with. (keep relatively low.)
    public float minimumCollisionOffset = 0.2f;

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

        defaultPosition = cameraTransform.localPosition.z;
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        targetTransform = ApplicationController.Instance.localPlayer.transform;
    }

    public void HandleAllCameraMovement()
    {
        if (targetTransform == null) return;

        FollowPlayer();
        RotateCamera();
        HandleCameraCollisions();
    } 

    private void FollowPlayer()
    {
        if (targetTransform == null) return;

        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;    
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = Mathf.Lerp(lookAngle, lookAngle + (inputManager.cameraInputX * cameraLookSpeed), camLookSmoothTime * Time.deltaTime);
        pivotAngle = Mathf.Lerp(pivotAngle, pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed), camLookSmoothTime * Time.deltaTime);

        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);
        if (lookAngle >= 360)
            lookAngle = 0;
        if (lookAngle <= -360)
            lookAngle = 0;

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;

        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);  //distance between our camera pivot and the thing we hit.
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        if (Mathf.Abs(cameraTransform.transform.localPosition.z) > Mathf.Abs(targetPosition))
            cameraVectorPosition.z = targetPosition;
        else
            cameraVectorPosition.z = Mathf.Lerp(cameraTransform.transform.localPosition.z, targetPosition, .2f);
        cameraTransform.localPosition = cameraVectorPosition;   
    }

}
