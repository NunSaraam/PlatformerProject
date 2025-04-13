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
                player.SetInvincible(true);     // ���� �ѱ�
                player.invincibleTimer = 1000f;    // ���� ���� �ð� ����
                Destroy(gameObject);            // ������ ����
            }
        }
    }

    private IEnumerator RemoveInvincibility(HeroKnight hero, float duration)
    {
        yield return new WaitForSeconds(duration);
        hero.SetInvincible(false);
    }
}
