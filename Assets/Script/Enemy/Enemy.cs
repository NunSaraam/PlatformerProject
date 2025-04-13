using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;

   public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name}�� {damage}�� ���ظ� ����. ����ü��:{health}");
    
        if ( health <= 0 )
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.tag = "Untagged";
        Destroy(gameObject);
    }
}
