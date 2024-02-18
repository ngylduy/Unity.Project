using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinachPassiveItem : PassiveItems
{
    protected override void ApplyEffect()
    {
        playerStats.currentMight *= 1 + passiveItemData.Multiplier / 100f;
    }
}
