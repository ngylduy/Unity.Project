using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive Item Table Object", menuName = "ScriptableObjects/PassiveItem")]
public class PassiveItemScripttableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; private set => multiplier = value; }

    [SerializeField]
    int level; //Not meant to be modified in the game
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab; //The prefab for the next level of the weapon
                                //Not to be confused with the prefab to be spawned at the next level
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite passiveItemIcon;
    public Sprite PassiveItemIcon { get => passiveItemIcon; private set => passiveItemIcon = value; }
}
