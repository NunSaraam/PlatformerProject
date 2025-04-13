using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 플레이어
    public float smoothSpeed = 5f; // 카메라 움직임 속도
    public Vector2 minPosition; // 카메라가 갈 수 있는 최소 위치 (왼쪽 아래)
    public Vector2 maxPosition; // 카메라가 갈 수 있는 최대 위치 (오른쪽 위)

    void LateUpdate()
    {
        if (target != null)
        {
            // 따라갈 위치 설정
            Vector3 desiredPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            // 카메라 위치 부드럽게 이동
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // x, y 위치를 맵 범위 안으로 제한
            float clampedX = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);

            transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
        }
    }
}