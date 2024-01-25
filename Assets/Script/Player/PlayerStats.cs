using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public CharacterScriptableObject characterData;

    //Current stats
    float currentMoveSpeed;
    float currentHealth;
    float currentRecovery;
    float currentProjectileSpeed;
    float currentMight;

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap = 100;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    //I-Frames
    public float iFrameDuration;
    float iFrameTimer;
    bool iFramesActive;

    public List<LevelRange> levelRanges;

    void Awake()
    {
        currentMoveSpeed = characterData.Speed;
        currentHealth = characterData.Health;
        currentRecovery = characterData.Recovery;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMight = characterData.Might;
    }

    void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
    }

    void Update()
    {
        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else if (iFramesActive)
        {
            iFramesActive = false;
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUp();
    }

    void LevelUp()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange levelRange in levelRanges)
            {
                if (level >= levelRange.startLevel && level <= levelRange.endLevel)
                {
                    experienceCapIncrease = levelRange.experienceCapIncrease;
                    break;
                }
            }

            experienceCap += experienceCapIncrease;
        }

    }

    public void TakeDamage(float damage)
    {
        if (!iFramesActive)
        {
            currentHealth -= damage;

            iFrameTimer = iFrameDuration;
            iFramesActive = true;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

    }

    void Die()
    {
        Debug.Log("Player died");
    }

    public void RestoreHealth(float amount)
    {
        if (currentHealth < characterData.Health)
        {
            currentHealth += amount;
            if (currentHealth > characterData.Health)
            {
                currentHealth = characterData.Health;
            }
        }

    }
}