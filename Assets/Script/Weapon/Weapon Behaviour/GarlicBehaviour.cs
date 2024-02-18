using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlicBehaviour : MeleWeaponBeahaviour
{

    List<GameObject> markedEnemies;

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag ("Enemy") && !markedEnemies.Contains(collision.gameObject))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(currentDamage);

            markedEnemies.Add(collision.gameObject); //Mark enemy so it dose not take damage another instance of dame from this garlic
        }
    }
}
