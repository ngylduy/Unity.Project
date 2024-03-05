using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItems> passiveItemsSlot = new List<PassiveItems>(6);
    public int[] passiveItemsLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);


    [Serializable]
    public class WeaponUpgrade
    {
        public int indexUpgradeWeapon;
        public GameObject initialWeapon;
        public WeaponTableObject weaponData;
    }

    [Serializable]
    public class PassiveItemUpgrade
    {
        public int indexUpgradePassiveItem;
        public GameObject initialPassiveItem;
        public PassiveItemScripttableObject passiveItemData;
    }

    [Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeName;
        public TMP_Text upgradeDescription;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgrades = new List<WeaponUpgrade>(); //List of weapon upgrades
    public List<PassiveItemUpgrade> passiveItemUpgrades = new List<PassiveItemUpgrade>(); //List of passive item upgrades
    public List<UpgradeUI> upgradeUIs = new List<UpgradeUI>(); //List of upgrade UIs

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true; //Enable the image component
        weaponUISlots[slotIndex].sprite = weapon.weaponData.WeaponIcon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void AddPassiveItem(int slotIndex, PassiveItems passiveItem)
    {
        passiveItemsSlot[slotIndex] = passiveItem;
        passiveItemsLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true; //Enable the image component
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.PassiveItemIcon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];

            if (!weapon.weaponData.NextLevelPrefab)
            { //Check if the weapon has a next level prefab
                Debug.LogError("No next level prefab for weapon" + weapon.name);
                return;
            }

            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); //Set the weapon to be a child of the player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;

            weaponUpgrades[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData; //To make sure have the correct weapon level

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemsSlot.Count > slotIndex)
        {
            PassiveItems passiveItems = passiveItemsSlot[slotIndex];

            if (!passiveItems.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("No next level prefab for passive item" + passiveItems.name);
                return;
            }

            GameObject upgradePassiveItem = Instantiate(passiveItems.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradePassiveItem.transform.SetParent(transform); //Set the weapon to be a child of the player
            AddPassiveItem(slotIndex, upgradePassiveItem.GetComponent<PassiveItems>());
            Destroy(passiveItems.gameObject);
            passiveItemsLevels[slotIndex] = upgradePassiveItem.GetComponent<PassiveItems>().passiveItemData.Level; //To make sure have the correct passive item level

            passiveItemUpgrades[upgradeIndex].passiveItemData = upgradePassiveItem.GetComponent<PassiveItems>().passiveItemData; //To make sure have the correct passive item level

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgrade()
    {

        List<WeaponUpgrade> aWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgrades);
        List<PassiveItemUpgrade> aPassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgrades);

        foreach (var upgradeOption in upgradeUIs)
        {

            if (aWeaponUpgrades.Count == 0 && aPassiveItemUpgrades.Count == 0)
            {
                return;
            }

            int upgradeType;

            if (aWeaponUpgrades.Count == 0)
            {
                upgradeType = 2;
            }
            else if (aPassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1;
            }
            else
            {
                upgradeType = UnityEngine.Random.Range(1, 3);
            }

            if (upgradeType == 1)
            {
                WeaponUpgrade choosenWeaponUpgrade = aWeaponUpgrades[UnityEngine.Random.Range(0, aWeaponUpgrades.Count)]; //Randomly select a weapon upgrade

                aWeaponUpgrades.Remove(choosenWeaponUpgrade); //Remove the choosen weapon upgrade from the list

                if (choosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == choosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {

                                if (!choosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, choosenWeaponUpgrade.indexUpgradeWeapon)); //Apply button functionality
                                //Set the description and name of the weapon upgrade
                                upgradeOption.upgradeDescription.text = choosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                                upgradeOption.upgradeName.text = choosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if (newWeapon) //Spawn the weapon if it is new
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnedWeapon(choosenWeaponUpgrade.initialWeapon));
                        //Apply initial weapon description and name
                        upgradeOption.upgradeDescription.text = choosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeName.text = choosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = choosenWeaponUpgrade.weaponData.WeaponIcon;
                }
            }
            else if (upgradeType == 2)
            {
                PassiveItemUpgrade choosenPassiveItemUpgrade = aPassiveItemUpgrades[UnityEngine.Random.Range(0, aPassiveItemUpgrades.Count)]; //Randomly select a passive item upgrade

                aPassiveItemUpgrades.Remove(choosenPassiveItemUpgrade); //Remove the choosen passive item upgrade from the list

                if (choosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemsSlot.Count; i++)
                    {
                        if (passiveItemsSlot[i] != null && passiveItemsSlot[i].passiveItemData == choosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;
                            if (!newPassiveItem)
                            {

                                if (!choosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, choosenPassiveItemUpgrade.indexUpgradePassiveItem)); //Apply button functionality

                                upgradeOption.upgradeDescription.text = choosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItems>().passiveItemData.Description;
                                upgradeOption.upgradeName.text = choosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItems>().passiveItemData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnedPassiveItem(choosenPassiveItemUpgrade.initialPassiveItem));
                        upgradeOption.upgradeDescription.text = choosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeName.text = choosenPassiveItemUpgrade.passiveItemData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = choosenPassiveItemUpgrade.passiveItemData.PassiveItemIcon;
                }
            }
        }
    }

    void RemoveUpgrade()
    {
        foreach (var upgradeOption in upgradeUIs)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption); //Display all ui options before applying the upgrade
        }
    }

    public void RemoveAndApplyUpgrade()
    {
        RemoveUpgrade();
        ApplyUpgrade();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeName.transform.parent.gameObject.SetActive(false);
    }
    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeName.transform.parent.gameObject.SetActive(true);
    }

}
