using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    public Animator animator;
    PlayerMoverment playerMoverment;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMoverment = GetComponent<PlayerMoverment>();
        spriteRenderer = GetComponent<SpriteRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }

    void Animate()
    {
        animator.SetFloat("MoveX", playerMoverment.moveDir.x);
        animator.SetFloat("MoveY", playerMoverment.moveDir.y);
        animator.SetFloat("Magnitude", playerMoverment.moveDir.magnitude);

        animator.SetFloat("LastMoveX", playerMoverment.lastMoveDir.x);
        animator.SetFloat("LastMoveY", playerMoverment.lastMoveDir.y);
        if (playerMoverment.moveDir.x < 0)
        {
            spriteRenderer.flipX = true;
        } else if (playerMoverment.moveDir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
}
