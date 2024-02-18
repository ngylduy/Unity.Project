using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    //Current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 10f; //Distance from player to despawn the enemy
    Transform player;

    void Awake()
    {
        currentMoveSpeed = enemyData.Speed;
        currentHealth = enemyData.Health;
        currentDamage = enemyData.Damage;
    }

    void Start() {
        player = FindObjectOfType<PlayerStats>().transform;
    }

    void Update() {
        if (Vector2.Distance(transform.position, player.position) > despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = col.gameObject.GetComponent<PlayerStats>();
            playerStats.TakeDamage(currentDamage);
        }
    }

    void OnDestroy() {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        enemySpawner.enemyKilled();
    }

    void ReturnEnemy()
    {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + enemySpawner.spawnPositions[Random.Range(0, enemySpawner.spawnPositions.Count)].position;
    }
}
