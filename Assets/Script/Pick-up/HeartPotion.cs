using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPotion : MonoBehaviour, ICollectible
{
    public int healthToRestore;

    public void Collect()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        playerStats.RestoreHealth(healthToRestore);
        Destroy(gameObject);
    }
}
