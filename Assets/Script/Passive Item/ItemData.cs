using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    [Serializable]
    public struct Evolution
    {
        public string name;
        public enum Condition { auto, treasureChest }
        public Condition condition;

        [System.Flags] public enum Consumption { passives = 1, weapons = 2 }
        public Consumption consumes;

        public int envolutionsLevel;
        public Config[] catalysts;
        public Config outcome;

        [Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }
    }
    public Evolution[] evolutionData;
}
