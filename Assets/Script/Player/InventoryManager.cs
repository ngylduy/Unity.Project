using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(6);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItems> passiveItemsSlot = new List<PassiveItems>(6);
    public int[] passiveItemsLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true; //Enable the image component
        weaponUISlots[slotIndex].sprite = weapon.weaponData.WeaponIcon;
    }

    public void AddPassiveItem(int slotIndex, PassiveItems passiveItem)
    {
        passiveItemsSlot[slotIndex] = passiveItem;
        passiveItemsLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true; //Enable the image component
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.PassiveItemIcon;
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if(weaponSlots.Count > slotIndex){
            WeaponController weapon = weaponSlots[slotIndex];

            if(!weapon.weaponData.NextLevelPrefab){ //Check if the weapon has a next level prefab
                Debug.LogError("No next level prefab for weapon" + weapon.name);
                return;
            }

            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); //Set the weapon to be a child of the player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;
        }
    }

    public void LevelUpPassiveItem(int slotIndex)
    {
        if(passiveItemsSlot.Count > slotIndex){
            PassiveItems passiveItems = passiveItemsSlot[slotIndex];

            if(!passiveItems.passiveItemData.NextLevelPrefab){
                Debug.LogError("No next level prefab for passive item" + passiveItems.name);
                return;
            }

            GameObject upgradePassiveItem = Instantiate(passiveItems.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradePassiveItem.transform.SetParent(transform); //Set the weapon to be a child of the player
            AddPassiveItem(slotIndex, upgradePassiveItem.GetComponent<PassiveItems>());
            Destroy(passiveItems.gameObject);
            weaponLevels[slotIndex] = upgradePassiveItem.GetComponent<PassiveItems>().passiveItemData.Level; //To make sure have the correct passive item level
        }
    }
}
