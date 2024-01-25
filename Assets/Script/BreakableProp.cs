using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableProp : MonoBehaviour
{
    public float health;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject.Destroy(gameObject);
    }
}
