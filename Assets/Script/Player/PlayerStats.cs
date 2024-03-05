using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{

    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    float health;


    #region Current stats properties
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value) //Check if the value is different
            {
                health = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthText.text = string.Format(
                        "Health: {0} / {1}", health, actualStats.maxHealth
                    );
                }
            }
        }
    }

    public float maxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value) //Check if the value is different
            {
                actualStats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthText.text = string.Format(
                        "Health: {0} / {1}", health, actualStats.maxHealth
                    );
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }
    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value) //Check if the value is different
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryText.text = "Recovery: " + actualStats.recovery;
                }
                //Update the real time value of the stat
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value) //Check if the value is different
            {
                actualStats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedText.text = "Move Speed: " + actualStats.moveSpeed;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }

    public float Might
    {
        get { return actualStats.might; }
        set
        {
            if (actualStats.might != value) //Check if the value is different
            {
                actualStats.might = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightText.text = "Might: " + actualStats.might;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }

    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            if (actualStats.speed != value) //Check if the value is different
            {
                actualStats.speed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedText.text = "Projectile Speed: " + actualStats.speed;
                }
                //Update the real time value of the stat
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }

    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value) //Check if the value is different
            {
                actualStats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetText.text = "Magnet: " + actualStats.magnet;
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

    PlayerInventory inventory;
    public int weaponSlotIndex;
    public int passiveItemSlotIndex;

    [Header("UI")]
    public Image healthBar;
    public Image experienceBar;
    public TMP_Text levelText;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySigleton();

        inventory = GetComponent<PlayerInventory>();

        //Assign the variables
        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;
    }

    void Start()
    {
        //Spawn the starting weapon
        inventory.Add(characterData.StartingWeapon);

        //Initialise the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;

        //Set the current stats display
        GameManager.instance.currentHealthText.text = "Health: " + CurrentHealth;
        GameManager.instance.currentMoveSpeedText.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentRecoveryText.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentProjectileSpeedText.text = "Projectile Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMightText.text = "Might: " + CurrentMight;
        GameManager.instance.currentMagnetText.text = "Magnet: " + CurrentMagnet;

        GameManager.instance.SetCharacterStats(characterData);

        UpdateHealthBar();
        UpdateExperienceBar();
        UpdateLevelText();
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

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;

        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUp();
        UpdateExperienceBar();
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
            UpdateLevelText();
            GameManager.instance.StartLevelUp();
        }

    }

    void UpdateExperienceBar()
    {
        experienceBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "Level: " + level.ToString();
    }

    public void TakeDamage(float damage)
    {
        if (!iFramesActive)
        {
            CurrentHealth -= damage;

            if (damegeEffect) Destroy(Instantiate(damegeEffect, transform.position, Quaternion.identity), 5f);

            iFrameTimer = iFrameDuration;
            iFramesActive = true;

            if (CurrentHealth <= 0)
            {
                Die();
            }

            UpdateHealthBar();
        }

    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    void Die()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.LevelReached(level);
            GameManager.instance.WeaponAndPassiveItem(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            CurrentHealth += Recovery * Time.deltaTime;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    [Obsolete("Old function tha is kept to maintian compatibility with the inventorymanager. Will be removed soon.")]
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
        //inventory.AddWeapon(weaponSlotIndex, spawnedWeapon.GetComponent<WeaponController>()); //Add weapon to inventory

        weaponSlotIndex++;
    }

    [Obsolete("No need to spawn passive items directly now.")]
    public void SpawnedPassiveItem(GameObject PassiveItem)
    {
        //Check if the PassiveItem slot index is at the max
        if (passiveItemSlotIndex >= inventory.passiveSlots.Count - 1) //-1 because the index starts at 0
        {
            Debug.LogError("PassiveItem slot index is at max");
            return;
        }

        //Spawn the starting PassiveItem
        GameObject spawnedPassiveItem = Instantiate(PassiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //Set PassiveItem to be a child of player
        //inventory.AddPassiveItem(passiveItemSlotIndex, spawnedPassiveItem.GetComponent<PassiveItems>()); //Add PassiveItem to inventory

        passiveItemSlotIndex++;
    }
}
