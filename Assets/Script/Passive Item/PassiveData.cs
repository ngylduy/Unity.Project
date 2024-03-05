using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive Data", menuName = "Vampire/Passive Data")]
public class PassiveData : ItemData
{
    public Passive.Modifer baseStats;
    public Passive.Modifer[] growthl;

    public Passive.Modifer GetLevelData(int level)
    {
        if (level - 2 < growthl.Length)
        {
            return growthl[level - 2];
        }

        Debug.LogWarning(string.Format("Passive dosen't have its level up stats configured for level {0}", level));
        return new Passive.Modifer();
    }
}
