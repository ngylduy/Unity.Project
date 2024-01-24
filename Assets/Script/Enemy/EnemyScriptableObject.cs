using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField]
    float health;
    public float Health { get => health; private set => health = value; }
    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }
}
