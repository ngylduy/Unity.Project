using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoverment : MonoBehaviour
{

    public EnemyScriptableObject enemyData;
    Transform player;

    Rigidbody2D rb;

    [HideInInspector]
    public Vector2 movement;
    [HideInInspector]
    public Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMoverment>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = player.position - transform.position;
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        moveDir.Normalize();
        movement = moveDir;

    }

    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (movement * enemyData.Speed * Time.fixedDeltaTime));
    }

}
