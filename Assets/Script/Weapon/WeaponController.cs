using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponTableObject weaponData;
    float cooldown;
    protected PlayerMoverment playerMoverment;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerMoverment = FindObjectOfType<PlayerMoverment>();
        cooldown = weaponData.CooldownDuration;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        cooldown = weaponData.CooldownDuration;
    }
}
