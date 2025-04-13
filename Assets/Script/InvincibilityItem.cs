using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HeroKnight player = other.GetComponent<HeroKnight>();
            if (player != null)
            {
                player.SetInvincible(true);     // 무적 켜기
                player.invincibleTimer = 1000f;    // 무적 지속 시간 설정
                Destroy(gameObject);            // 아이템 제거
            }
        }
    }

    private IEnumerator RemoveInvincibility(HeroKnight hero, float duration)
    {
        yield return new WaitForSeconds(duration);
        hero.SetInvincible(false);
    }
}
