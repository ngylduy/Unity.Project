using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();

        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0)
            {
                Attack(currentAttackCount);
            }
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0)
        {
            return true;
        }
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        //If no projectile prefab is assigned, leave a warning message
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0} ", name));
            currentCooldown = data.baseStats.cooldwon;
            return false;
        }

        //Can we attack?
        if (!CanAttack())
        {
            return false;
        }

        //Orhterwise, calculate the angle and offset of our spawned projectile
        float spawnAngle = GetSpawnAngle();

        //And spawn a copy of the projectile
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );

        prefab.weapon = this;
        prefab.owner = owner;

        //Rreset the cooldown only if this attack was triggered by cooldown
        if (currentCooldown <= 0)
        {
            currentCooldown += currentStats.cooldwon;
        }

        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

    //Get which direction the projectile should face when spawning
    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(moverment.lastMoveDir.y, moverment.lastMoveDir.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            UnityEngine.Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            UnityEngine.Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
