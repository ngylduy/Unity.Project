using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource
    {
        projectile, owner
    }
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public UnityEngine.Vector3 rotationSpeed = new UnityEngine.Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }

        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new UnityEngine.Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        //Set how much piercing this object has
        piercing = stats.priercing;

        if (stats.lifespan > 0)
        {
            Destroy(gameObject, stats.lifespan);
        }
        if (hasAutoAim)
        {
            AcquireAutoAimFacing();
        }
    }

    //If the projectile is homing, it will automatically find a suiable enemy to target
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; //Need to determine where to aim
        
        //Find all enemies in the scene
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        //Select a random enemy if there is at least one
        //Otherwise, just aim in a random direction
        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
            UnityEngine.Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        } else {
            aimAngle = UnityEngine.Random.Range(0f, 360f);
        }

        //Point the projectile in the direction of the selected enemy
        transform.rotation = UnityEngine.Quaternion.Euler(0, 0, aimAngle);
    }

    protected virtual void FixedUpdate() {
        //Only drice movement ourseles if this is kinematic
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other){

        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProp p = other.GetComponent<BreakableProp>();

        if (es)
        {
            //If there is an owner, and the damage source is set to owner
            //Will calculate knockback using the owner instead of the projectile
            UnityEngine.Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            
            //Deal damage and destroys the projectile
            es.TakeDamage(GetDamage(), source);
            
            Weapon.Stats stats = weapon.GetStats();
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, UnityEngine.Quaternion.identity), 5f);
            }

        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, UnityEngine.Quaternion.identity), 5f);
            }
        }

        //Destroy this object if it has run out of health from hitting other stuff.
        if (piercing <= 0)
        {
            Destroy(gameObject);
        }
    }
}