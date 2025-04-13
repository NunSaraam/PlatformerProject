using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;

   public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name}가 {damage}의 피해를 입음. 남은체력:{health}");
    
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
