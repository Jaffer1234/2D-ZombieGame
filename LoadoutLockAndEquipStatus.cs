using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutLockAndEquipStatus : MonoBehaviour
{
    [Header("Locked and Unlocked Panels")]

    public int myWeaponNumberIs;
    public int myLoadoutNumberIs;

    public GameObject lockedPanel;
    public GameObject unlockedPanel;

    [Header("Equip and Equipped")]

    public GameObject equip;
    public GameObject equipped;


    [Header("Player Attachments")]
    public int weaponIndex;
    public GameObject attachments;


    private string _prefKey;
    public string prefKey { get { return _prefKey; } }
    private string _prefEquip;
    public string prefEquip { get { return _prefEquip; } }
    private int _loadoutNumber = 0;
    public int loadoutNumber { get { return _loadoutNumber; } }
    private int _weaponNumber = 0;
    public int weaponNumber { get { return _weaponNumber; } }

    // Use this for initialization
    void Awake()
    {
        Loadouts loadout = GetComponentInParent<Loadouts>() as Loadouts;

        if (loadout)
        {
            _weaponNumber = myWeaponNumberIs;
            _loadoutNumber = myLoadoutNumberIs;


            _prefKey = "Loadout" + loadoutNumber + "Weapon" + weaponNumber;
            _prefEquip = "Loadout" + loadoutNumber + "Equipped";
        }
        else
            Utility.ErrorLog("Loadouts Component not found in parent of " + this.gameObject.name, 2);
    }
    public void OnEnable()
    {
        if (prefKey != null)
        {
            if (lockedPanel)
            {
                lockedPanel.SetActive(false);
            }
            else
                Utility.ErrorLog("Locked Panel is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);

            if (unlockedPanel)
            {
                unlockedPanel.SetActive(false);
            }
            else
                Utility.ErrorLog("Unlocked Panel is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);

            if (EncryptedPlayerPrefs.GetInt(prefKey) == 0)
            {
                if (lockedPanel)
                {
                    lockedPanel.SetActive(true);
                }
                else
                    Utility.ErrorLog("Locked Panel is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);
            }
            else if (EncryptedPlayerPrefs.GetInt(prefKey) == 1)
            {
                if (unlockedPanel)
                {
                    unlockedPanel.SetActive(true);
                }
                else
                    Utility.ErrorLog("Unlocked Panel is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);
            }


            if (equip)
            {
                equip.SetActive(false);
            }
            else
                Utility.ErrorLog("Equip is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);
            if (equipped)
            {
                equipped.SetActive(false);
            }
            else
                Utility.ErrorLog("Equipped is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);

            if (EncryptedPlayerPrefs.GetInt(_prefEquip) == _weaponNumber)
            {
                if (equipped)
                {
                    equipped.SetActive(true);
                    //playerAttachments.GetComponent<EnableSelectedWeapon>().EnableWeapons(weaponIndex);ni 

                }
                else
                    Utility.ErrorLog("Equip is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);
            }
            else
            {
                if (equip)
                {
                    equip.SetActive(true);
                }
                else
                    Utility.ErrorLog("Equipped is not assigned in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);
            }
        }
        else
            Utility.ErrorLog("prefKey is null in LoadoutLockAndEquipStatus.cs of " + this.gameObject.name, 1);
    }
}