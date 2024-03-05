using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


/// <summary>
/// Component to be attached to all weapon prefabs. The weapon prefab works together with the weapondata
/// ScriptableObjects to manage and run the behaviour of all weapon in the game.
/// </summary>
public abstract class Weapon : Item
{
    [Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Virtual")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; //If 0, it will last forever
        //Damage <= Actual damage <= Damage + DamageVariance
        public float damage, damageVariance, area, speed, cooldwon, projectileInterval, knockback;
        public int number, priercing, maxInstances;

        //Allows us to use the + operator to add two stats together
        //Very important later when we want to increase the stats of a weapon
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldwon = s1.cooldwon + s2.cooldwon;
            result.number = s1.number + s2.number;
            result.priercing = s1.priercing + s2.priercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result;
        }

        public float GetDame()
        {
            return damage + UnityEngine.Random.Range(0, damageVariance);
        }
    }
    protected Stats currentStats;
    public WeaponData data;
    protected float currentCooldown;
    protected PlayerMoverment moverment; //Reference to the player's moverment script

    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        moverment = GetComponentInParent<PlayerMoverment>();
        currentCooldown = currentStats.cooldwon;
    }

    protected virtual void Awake()
    {
        if (data)
        {
            currentStats = data.baseStats;
        }
    }

    protected virtual void Start()
    {
        if (data)
        {
            Initialise(data);
        }
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack(currentStats.number);
        }
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached", name, currentLevel, data.maxLevel));
            return false;
        }
        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }

    public virtual bool CanAttack()
    {
        return currentCooldown <= 0f;
    }

    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldwon;
            return true;
        }
        return false;
    }

    public virtual float GetDamage()
    {
        return currentStats.GetDame() * owner.CurrentMight;
    }

    public virtual Stats GetStats()
    {
        return currentStats;
    }
}
