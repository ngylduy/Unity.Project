using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("This will be replaced by the WeaponData class.")]
[CreateAssetMenu(fileName = "WeaponTableObject", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponTableObject : ScriptableObject
{
    [SerializeField]
    GameObject prefab;
    //Base Stats for weapon
    public GameObject Prefab { get => prefab; private set => prefab = value;}
    [SerializeField]
    float dame;
    public float Dame { get => dame; private set => dame = value; }
    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }
    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }
    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }

    [SerializeField]
    int level; //Not meant to be modified in the game
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab; //The prefab for the next level of the weapon
                                //Not to be confused with the prefab to be spawned at the next level
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description; //Description of the weapon
    public string Description { get => description; private set => description = value; }

    [SerializeField]
    Sprite weaponIcon;
    public Sprite WeaponIcon { get => weaponIcon; private set => weaponIcon = value; }

}
