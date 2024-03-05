using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base script for all mele weapons
/// </summary>
public class MeleWeaponBeahaviour : MonoBehaviour
{
    public WeaponTableObject weaponData;

    public float destroyAfterSecond;

    //Current Stats
    protected float currentDamage;
    protected int currentPierce;
    protected float currentSpeed;
    protected float currentCooldownDuration;

    void Awake()
    {
        currentDamage = weaponData.Dame;
        currentPierce = weaponData.Pierce;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
    }

    public float GetCurrentDame()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentMight;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSecond);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(GetCurrentDame(), transform.position);
        }
        else if (other.CompareTag("Prop"))
        {
            if (other.gameObject.TryGetComponent(out BreakableProp breakableProp))
            {
                breakableProp.TakeDamage(GetCurrentDame());
            }
        }
    }
}
