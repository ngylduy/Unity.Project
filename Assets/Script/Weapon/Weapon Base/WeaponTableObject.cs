using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
