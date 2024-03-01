using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    CharacterScriptableObject characterData;

    //Current stats
    float currentMoveSpeed;
    float currentHealth;
    float currentRecovery;
    float currentProjectileSpeed;
    float currentMight;
    float currentMagnet;

    #region Current stats properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value) //Check if the value is different
            {
                currentHealth = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthText.text = "Health: " + currentHealth;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value) //Check if the value is different
            {
                currentMoveSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedText.text = "Move Speed: " + currentMoveSpeed;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value) //Check if the value is different
            {
                currentRecovery = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryText.text = "Recovery: " + currentRecovery;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value) //Check if the value is different
            {
                currentProjectileSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedText.text = "Projectile Speed: " + currentProjectileSpeed;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if (currentMight != value) //Check if the value is different
            {
                currentMight = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMightText.text = "Might: " + currentMight;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value) //Check if the value is different
            {
                currentMagnet = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetText.text = "Magnet: " + currentMagnet;
                }
                //Update the real time value of the stat
            }
        }
    }
    #endregion

    public ParticleSystem damegeEffect;

    //Experience/Level of the player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap = 100;

    //Class for defining the level ranges and the corresponding experience cap increase for the range
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

    InventoryManager inventory;
    public int weaponSlotIndex;
    public int passiveItemSlotIndex;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySigleton();

        inventory = GetComponent<InventoryManager>();

        CurrentMoveSpeed = characterData.Speed;
        CurrentHealth = characterData.Health;
        CurrentRecovery = characterData.Recovery;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMight = characterData.Might;
        CurrentMagnet = characterData.Magnet;

        SpawnedWeapon(characterData.StatingWeapon);
    }

    void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;

        //Set the current stats display
        GameManager.instance.currentHealthText.text = "Health: " + currentHealth;
        GameManager.instance.currentMoveSpeedText.text = "Move Speed: " + currentMoveSpeed;
        GameManager.instance.currentRecoveryText.text = "Recovery: " + currentRecovery;
        GameManager.instance.currentProjectileSpeedText.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMightText.text = "Might: " + currentMight;
        GameManager.instance.currentMagnetText.text = "Magnet: " + currentMagnet;
    
        GameManager.instance.SetCharacterStats(characterData);
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
            GameManager.instance.StartLevelUp();
        }

    }

    public void TakeDamage(float damage)
    {
        if (!iFramesActive)
        {
            CurrentHealth -= damage;

            if(damegeEffect) Instantiate(damegeEffect, transform.position, Quaternion.identity);

            iFrameTimer = iFrameDuration;
            iFramesActive = true;

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

    }

    void Die()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.LevelReached(level);
            GameManager.instance.WeaponAndPassiveItem(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < characterData.Health)
        {
            CurrentHealth += amount;
            if (CurrentHealth > characterData.Health)
            {
                CurrentHealth = characterData.Health;
            }
        }
    }

    void Recover()
    {
        if (CurrentHealth < characterData.Health)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > characterData.Health)
            {
                CurrentHealth = characterData.Health;
            }
        }
    }

    public void SpawnedWeapon(GameObject weapon)
    {
        //Check if the weapon slot index is at the max
        if (weaponSlotIndex >= inventory.weaponSlots.Count - 1) //-1 because the index starts at 0
        {
            Debug.LogError("Weapon slot index is at max");
            return;
        }

        //Spawn the starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); //Set weapon to be a child of player
        inventory.AddWeapon(weaponSlotIndex, spawnedWeapon.GetComponent<WeaponController>()); //Add weapon to inventory

        weaponSlotIndex++;
    }
    public void SpawnedPassiveItem(GameObject PassiveItem)
    {
        //Check if the PassiveItem slot index is at the max
        if (passiveItemSlotIndex >= inventory.passiveItemsSlot.Count - 1) //-1 because the index starts at 0
        {
            Debug.LogError("PassiveItem slot index is at max");
            return;
        }

        //Spawn the starting PassiveItem
        GameObject spawnedPassiveItem = Instantiate(PassiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //Set PassiveItem to be a child of player
        inventory.AddPassiveItem(passiveItemSlotIndex, spawnedPassiveItem.GetComponent<PassiveItems>()); //Add PassiveItem to inventory

        passiveItemSlotIndex++;
    }
}
