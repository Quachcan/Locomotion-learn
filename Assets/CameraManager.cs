using System;
using System.Numerics;
using MyProject.Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraManager : MonoBehaviour
{
    private InputManager inputManager;
    
    
    public Transform targetTransform; //Object the camera will follow 
    public Transform cameraPivot;   //Object the camera uses pivot
    private Transform cameraTransform;
    public LayerMask collisionLayer;
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffSet = 0.2f;
    public float minimumCollisionOffSet = 0.2f;
    public float cameraCollisionRadius = 2;
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 1f;
    public float cameraPivotSpeed = 1f;

    public float lookAngle; //Camera looking up and down
    public float pivotAngle;    //Camera looking left and right
    public float minimumPivotAngle = -35f;
    public float maximumPivotAngle = 35f;
    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        targetTransform = FindFirstObjectByType<PlayerManager>().transform;
        cameraTransform = Camera.main?.transform;
        if (cameraTransform != null) defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollision();
    }
    
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollision()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();
        
        if(Physics.SphereCast
               (cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }
        
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
    
    private void OnDrawGizmos()
    {
        if (cameraPivot == null || cameraTransform == null)
            return;

        // Tính toán hướng từ cameraPivot đến cameraTransform
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        // Tính toán khoảng cách tối đa
        float maxDistance = Mathf.Abs(defaultPosition);

        // Vẽ sphere ở cameraPivot để minh họa bắt đầu của SphereCast
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cameraPivot.position, cameraCollisionRadius);

        // Vẽ ray hướng ra từ cameraPivot
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPivot.position, direction * maxDistance);

        // Nếu phát hiện va chạm, vẽ một sphere tại điểm va chạm
        RaycastHit hit;
        if (Physics.SphereCast(cameraPivot.position, cameraCollisionRadius, direction, out hit, maxDistance, collisionLayer))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, cameraCollisionRadius * 0.5f);
        }
    }
    
}
