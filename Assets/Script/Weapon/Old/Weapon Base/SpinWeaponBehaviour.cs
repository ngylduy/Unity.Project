using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWeaponBehaviour : MonoBehaviour
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

    // Update is called once per frame
    void Update()
    {

    }
}
