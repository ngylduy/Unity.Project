using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoverment : MonoBehaviour
{

    public float speed;
    Rigidbody2D rb;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMoveDir;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (moveDir.x != 0 || moveDir.y != 0)
        {
            lastMoveDir = moveDir;
        }

        moveDir = new Vector2(x, y).normalized;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
    }
}
