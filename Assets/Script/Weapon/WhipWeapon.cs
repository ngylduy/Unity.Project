using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount;
    float currentSpawnYOffset;

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("No projectile prefab found for {0}", name));
            currentCooldown = data.baseStats.cooldwon;
            return false;
        }

        if (!CanAttack())
        {
            return false;
        }

        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0;
        }

        float spawnDir = Mathf.Sign(moverment.lastMoveDir.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        UnityEngine.Vector2 spawnOffset = new UnityEngine.Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
        );

        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (UnityEngine.Vector3)spawnOffset,
            UnityEngine.Quaternion.identity
        );

        prefab.owner = owner;

        if (spawnDir < 0)
        {
            prefab.transform.localScale = new UnityEngine.Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
            );
            Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        prefab.weapon = this;
        currentCooldown = data.baseStats.cooldwon;
        attackCount--;

        currentSpawnCount++;

        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
        {
            currentSpawnYOffset += 1;
        }

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }
}
