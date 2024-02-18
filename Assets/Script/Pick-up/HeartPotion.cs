using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPotion : Pickup, ICollectible
{
    public int healthToRestore;

    public void Collect()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.RestoreHealth(healthToRestore);
    }
}
