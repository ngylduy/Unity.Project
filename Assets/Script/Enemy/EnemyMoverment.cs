using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoverment : MonoBehaviour
{

    EnemyStats enemyStats;
    Transform player;

    Vector2 knockbackVelocity;
    float knockbackDuration;


    Rigidbody2D rb;

    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMoverment>().transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyStats.currentMoveSpeed * Time.deltaTime);
        }

        moveDir = player.position - transform.position;
        //float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        moveDir.Normalize();
        movement = moveDir;

    }

    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (movement * enemyStats.currentMoveSpeed * Time.fixedDeltaTime));
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        //Ignore knockback if already being knocked back
        if (knockbackDuration > 0)
        {
            return;
        }
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

}
