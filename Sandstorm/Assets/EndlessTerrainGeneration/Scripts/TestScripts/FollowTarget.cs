using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offsetFromTarget;
    public float smoothTime = 0.25f;
    public bool useTargetLocalOrientationForPositionOffset = true;
    public bool lookAtTarget = true;
    public bool startAtDesiredPosition = true;

    private Transform t;
    private Vector3 refVelocity;

    private void Start()
    {
        t = transform;
        refVelocity = Vector3.zero;

        if (startAtDesiredPosition)
        {
            t.position = CalculateDesiredPosition();

            if (lookAtTarget)
            {
                t.LookAt(target);
            }
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            t.position = Vector3.SmoothDamp(t.position, CalculateDesiredPosition(), ref refVelocity, smoothTime);

            if (lookAtTarget)
            {
                t.LookAt(target);
            }
        }
    }

    private Vector3 CalculateDesiredPosition()
    {
        Vector3 desiredPosition = (useTargetLocalOrientationForPositionOffset)
                ? target.position + target.right * offsetFromTarget.x + target.up * offsetFromTarget.y
                                  + target.forward * offsetFromTarget.z
                : target.position + offsetFromTarget;

        return desiredPosition;
    }
}
