using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracePatrolController : MonoBehaviour
{
    public float patrolSpeed = 1f;
    public float chaseSpeed = 4f;
    public float traceDistance = 4f;
    public float raycastDistance = 1f;
    public Transform groundCheck; // 발밑 체크 포인트 (없으면 제거 가능)

    private Transform player;
    private bool isChasing = false;
    private bool isMovingRight = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isChasing = distanceToPlayer <= traceDistance;

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        float moveDirection = isMovingRight ? 1f : -1f;
        Vector2 move = new Vector2(moveDirection * patrolSpeed * Time.deltaTime, 0f);
        transform.Translate(move);

        Vector2 rayOrigin = transform.position;
        Vector2 rayDir = isMovingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, raycastDistance);
        Debug.DrawRay(rayOrigin, rayDir * raycastDistance, Color.yellow);

        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
        {
            Flip();
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        // 장애물 감지
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance);
        Debug.DrawRay(transform.position, direction * raycastDistance, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
        {
            // 장애물 있으면 방향 전환
            Flip();
            return;
        }

        // 플레이어 쪽으로 이동
        transform.Translate(direction * chaseSpeed * Time.deltaTime);

        // 시선도 플레이어 쪽으로
        if (direction.x > 0 && !isMovingRight) Flip();
        else if (direction.x < 0 && isMovingRight) Flip();
    }

    private void Flip()
    {
        isMovingRight = !isMovingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // 좌우 반전
        transform.localScale = localScale;
    }
}