using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerInventory : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignItem)
        {
            item = assignItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty()
        {
            return item == null;
        }
    }

    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>(); //List of upgrade otiopns for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); //List of upgrade options for passives
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(); //List of UI for upgrade options present in the scene

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    public bool Has(ItemData type) { return Get(type); }
    public Item Get(ItemData type)
    {
        if (type is WeaponData)
        {
            return Get(type as WeaponData);
        }
        else if (type is PassiveData)
        {
            return Get(type as PassiveData);
        }
        return null;
    }

    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
            {
                return p;
            }
        }
        return null;
    }

    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
            {
                return w;
            }
        }
        return null;
    }

    public bool Remove(WeaponData data, bool removeUpgradeAvailavility = false)
    {
        if (removeUpgradeAvailavility)
        {
            availableWeapons.Remove(data);
        }

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }

    public bool Remove(PassiveData data, bool removeUpgradeAvailavility = false)
    {
        if (removeUpgradeAvailavility)
        {
            availablePassives.Remove(data);
        }

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;
    }

    public bool Remove(ItemData data, bool removeUpgradeAvailavility = false)
    {
        if (data is WeaponData)
        {
            return Remove(data as WeaponData, removeUpgradeAvailavility);
        }
        else if (data is PassiveData)
        {
            return Remove(data as PassiveData, removeUpgradeAvailavility);
        }
        return false;
    }

    public int Add(WeaponData data)
    {
        int slotNum = -1;

        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        //If there is no empty slot, exit
        if (slotNum < 0)
        {
            return slotNum;
        }

        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);

            spawnedWeapon.Initialise(data);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("No weapon behaviour found for {0}", data.name));
        }
        return -1;
    }

    public int Add(PassiveData data)
    {
        int slotNum = -1;

        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        //If there is no empty slot, exit
        if (slotNum < 0)
        {
            return slotNum;
        }


        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(p);

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
        return slotNum;
    }

    public int Add(ItemData data)
    {
        if (data is WeaponData)
        {
            return Add(data as WeaponData);
        }
        else if (data is PassiveData)
        {
            return Add(data as PassiveData);
        }
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;
            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up weapon {0}", weapon.name));
                return;
            }
        }
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive passive = passiveSlots[slotIndex].item as Passive;
            if (!passive.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up passive {0}", passive.name));
                return;
            }
        }
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
    }

    void ApplyUpgradeOptions()
    {
        //Make a duplicate of the available weapons and passives
        List<WeaponData> availableWeaponUpgrades = new List<WeaponData>(availableWeapons);
        List<PassiveData> availablePassiveUpgrades = new List<PassiveData>(availablePassives);

        foreach (UpgradeUI upgradeOptions in upgradeUIOptions)
        {
            //If there are no more upgrades available, exit
            if (availableWeaponUpgrades.Count == 0 && availablePassiveUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (availablePassiveUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            if (upgradeType == 1)
            {
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOptions);

                    bool isLevelUp = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            //If the weapon has reached its max level, disable the upgrade UI
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOptions);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOptions.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOptions.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOptions.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOptions.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOptions.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade));
                        upgradeOptions.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOptions.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOptions.upgradeIcon.sprite = chosenWeaponUpgrade.icon;

                    }
                }
            }
            else if (upgradeType == 2)
            {
                PassiveData chosenPassiveUpgrade = availablePassiveUpgrades[UnityEngine.Random.Range(0, availablePassiveUpgrades.Count)];
                availablePassives.Remove(chosenPassiveUpgrade);

                if (chosenPassiveUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOptions);

                    bool isLevelUp = false;
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOptions);
                                isLevelUp = true;
                                break;
                            }

                            upgradeOptions.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
                            Passive.Modifer nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOptions.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOptions.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOptions.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }

                    if (!isLevelUp)
                    {
                        upgradeOptions.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade));
                        Passive.Modifer nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOptions.upgradeNameDisplay.text = nextLevel.name;
                        upgradeOptions.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOptions.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrade()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

}

