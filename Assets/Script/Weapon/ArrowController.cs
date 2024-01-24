using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedArrow = Instantiate(weaponData.Prefab);
        spawnedArrow.transform.position = transform.position;
        spawnedArrow.GetComponent<ArrowBehaviour>().DirectionChecker(playerMoverment.lastMoveDir);
    }
}
