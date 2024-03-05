using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingPassiveItem : PassiveItems
{
    protected override void ApplyEffect()
    {
        playerStats.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f;
    }
}
