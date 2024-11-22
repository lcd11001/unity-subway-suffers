using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The target the camera will follow
    public Vector3 followOffset; // The offset at which the camera will follow the target
    public Vector3 lookAtOffset; // The offset at which the camera will look at the target
    public float smoothSpeed = 0.2f; // The speed with which the camera will smooth its movement
    public float castLength = 2.5f; // The length of the raycast to check if the player is grounded

    public float lastFollowY;

    void OnValidate()
    {
        // this function is called when the script is loaded or a value is changed ONLY in the inspector
        if (target != null)
        {
            lastFollowY = transform.position.y;
            transform.position = target.position + followOffset;
            transform.LookAt(target.position + lookAtOffset);
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowCamera: No target set for the camera to follow.");
            return;
        }

        Vector3 desiredPosition = target.position + followOffset;

        // Check if the player is grounded
        if (IsGrounded())
        {
            //Debug.Log("Grounded");
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            transform.LookAt(target.position + lookAtOffset);

            // Update last grounded position and rotation
            lastFollowY = transform.position.y;
        }
        else
        {
            //Debug.Log("Not Grounded");
            // Keep the last grounded position and rotation
            transform.position = new Vector3(desiredPosition.x, lastFollowY, desiredPosition.z);
        }
    }

    private bool IsGrounded()
    {
        // target already not null
        // last check from LateUpdate

        RaycastHit hit;
        if (Physics.Raycast(target.position, Vector3.down, out hit, castLength))
        {
            return true;
        }
        return false;
    }
}
