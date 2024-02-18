using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnymation : MonoBehaviour
{
    public Animator animator;
    EnemyMoverment enemyMoverment;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyMoverment = GetComponent<EnemyMoverment>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
        UpdateSpriteFlip();
    }

    void Animate()
    {
        animator.SetFloat("MoveX", enemyMoverment.moveDir.x);
        animator.SetFloat("MoveY", enemyMoverment.moveDir.y);
        animator.SetFloat("Magnitude", enemyMoverment.moveDir.magnitude);
    }

    void UpdateSpriteFlip()
    {
        if (enemyMoverment.moveDir.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (enemyMoverment.moveDir.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

}
