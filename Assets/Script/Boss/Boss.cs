using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;

    public Transform player;
    public float moveSpeed = 3f;
    public float attackRange = 1f;

    public int maxHealth = 10;
    public int currenHealth;
    public Slider bossHealthSlider;

    public float teleportInterval = 5f;
    public Transform[] teleportPoints;
    private float teleportTimer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false;

    private bool isInvincible = false;
    public float invincibleDuration = 1f;

    private void Start()
    {
        currenHealth = maxHealth;
        bossHealthSlider.maxValue = maxHealth;
        bossHealthSlider.value = currenHealth;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bossHealthSlider.value = currenHealth;

        teleportTimer -= Time.deltaTime;
        if (teleportTimer <= 0f)
        {
            TeleportNearPlayer();
            teleportTimer = teleportInterval;
        }

        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);

        if (horizontalDistance <= attackRange && verticalDistance < 1f)
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    rb.velocity = Vector2.zero;
                    animator.SetTrigger("Attack2");
                }
            }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            animator.SetBool("IsRunning", true);
        }

        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-5, 5, 1);
        }
        else
        {
            transform.localScale = new Vector3(5, 5, 1);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsRunning",false);
    }
    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }
    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }

    void TeleportNearPlayer()
    {
        if (teleportPoints.Length == 0) return;
        Transform closest = teleportPoints[0];
        float minDistance = Vector2.Distance(player.position, closest.position);

        foreach (Transform point in teleportPoints)
        {
            float dist = Vector2.Distance(player.position, point.position);
            if (dist < minDistance)
            {
                closest = point;
                minDistance = dist;
            }
        }

        transform.position = closest.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Player"))
        {
            HeroKnight player = collision.GetComponent<HeroKnight>();
            if (player != null && !player.IsInvincible)
            {
                player.TriggerGameOver();
            }
            
        }
    }
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currenHealth -= damage;
            bossHealthSlider.value = currenHealth;
            if (currenHealth == 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(InvincibilityAndTeleport());
            }
        }
    }
    private IEnumerator InvincibilityAndTeleport()
    {
        isInvincible = true;
        animator.SetTrigger("IsInvincible");

        yield return new WaitForSeconds(invincibleDuration);

        if (teleportPoints.Length > 0)
        {
            Transform randomPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];
            transform.position = randomPoint.position;
        }
        teleportTimer = teleportInterval;

        isInvincible = false;
    }

    private bool isDead = false;

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("보스 처치!");
        animator.SetTrigger("Die");
        
        rb.velocity = Vector2.zero;
        animator.SetBool("IsRunning", false);

        bossHealthSlider.gameObject.SetActive(false);

        this.enabled = false;
    }
}
