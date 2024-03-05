using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPotion : Pickup
{
    public int healthToRestore;

    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.RestoreHealth(healthToRestore);
    }
}
