using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ���� �÷��̾�
    public float smoothSpeed = 5f; // ī�޶� ������ �ӵ�
    public Vector2 minPosition; // ī�޶� �� �� �ִ� �ּ� ��ġ (���� �Ʒ�)
    public Vector2 maxPosition; // ī�޶� �� �� �ִ� �ִ� ��ġ (������ ��)

    void LateUpdate()
    {
        if (target != null)
        {
            // ���� ��ġ ����
            Vector3 desiredPosition = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );

            // ī�޶� ��ġ �ε巴�� �̵�
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // x, y ��ġ�� �� ���� ������ ����
            float clampedX = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);

            transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
        }
    }
}