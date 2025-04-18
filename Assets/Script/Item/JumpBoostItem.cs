using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostItem : MonoBehaviour
{
    public float jumpBoostAmount = 7f;
    public float boostDuration = 1000f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HeroKnight player = collision.GetComponent<HeroKnight>();

            if (player != null)
            {
                player.ApplyJumpBoost(jumpBoostAmount, boostDuration);
            }
            Destroy(gameObject);
        }
    }
}
