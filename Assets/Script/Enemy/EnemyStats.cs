using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
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

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageColorDuration = 0.2f;
    public float deathFadeTime = 0.5f;
    Color originalColor;
    SpriteRenderer spriteRenderer;
    EnemyMoverment enemyMoverment;
    
    void Awake()
    {
        currentMoveSpeed = enemyData.Speed;
        currentHealth = enemyData.Health;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        enemyMoverment = GetComponent<EnemyMoverment>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) > despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float damage, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (damage > 0)
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(damage).ToString(), transform);
        }

        if (knockbackForce > 0)
        {
            Vector2 knockbackDir = (Vector2)transform.position - sourcePosition;
            enemyMoverment.Knockback(knockbackDir.normalized * knockbackForce, knockbackDuration);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //This is a coroutine that will make the sprite flash a color when the enemy takes damage
    IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageColorDuration);
        spriteRenderer.color = originalColor;
    }

    public void Die()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = spriteRenderer.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = col.gameObject.GetComponent<PlayerStats>();
            playerStats.TakeDamage(currentDamage);
        }
    }

    void OnDestroy()
    {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        enemySpawner.enemyKilled();
    }

    void ReturnEnemy()
    {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + enemySpawner.spawnPositions[UnityEngine.Random.Range(0, enemySpawner.spawnPositions.Count)].position;
    }
}
