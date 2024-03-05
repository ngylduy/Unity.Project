using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItems : MonoBehaviour
{
    protected PlayerStats playerStats;
    public PassiveItemScripttableObject passiveItemData;

    protected virtual void ApplyEffect()
    {
        //Apply boost value to the appropriate stat in the child class
    }

    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        ApplyEffect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
