using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector2 minPos;
    public Vector2 maxPos;


    void Update()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position;
            desiredPosition.z = transform.position.z;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPos.x, maxPos.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPos.y, maxPos.y);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
