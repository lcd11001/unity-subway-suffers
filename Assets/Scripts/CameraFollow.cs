using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow
    public Vector3 followOffset; // The offset at which the camera will follow the target
    public Vector3 lookAtOffset; // The offset at which the camera will look at the target
    public float smoothSpeed = 0.2f; // The speed with which the camera will smooth its movement

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowCamera: No target set for the camera to follow.");
            return;
        }

        Vector3 desiredPosition = target.position + followOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target.position + lookAtOffset);
    }
}
