using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutManager : MonoBehaviour
{
    [Header("Buttons Activators")]
    public GameObject[] panelButtonActivators;

    [Header("Loadout Types")]
    public Loadouts[] loadouts;

    public GameObject buyPanel;

    public void Start()
    {
        buyPanel.SetActive(false);
        OpenLoadout(1);
    }

    public void OpenLoadout(int index)
    {

        if (TutorialManager.isTutorialRunning)
        {
            if (EncryptedPlayerPrefs.GetInt("Tutorial") == 7)
            {
                TutorialManager.Instance.NextPanel();
            }
        }
        foreach (var item in panelButtonActivators)
        {
            if (item)
            {
                item.SetActive(false);
            }
            else
                Utility.ErrorLog("Panel Button Activators is not assigned in ItemShopManager.cs of " + this.gameObject, 1);
        }

        if (index < panelButtonActivators.Length)
        {
            if (panelButtonActivators[index])
            {
                panelButtonActivators[index].SetActive(true);
            }
            else
                Utility.ErrorLog("Panel Button Activators is not assigned in ItemShopManager.cs of " + this.gameObject, 1);
        }
        else
            Utility.ErrorLog("Array out of bound of Panel Button Activators index in ItemShopManager.cs " + " of " + this.gameObject.name, 4);



        foreach (var item in loadouts)
        {
            if (item)
            {
                item.gameObject.SetActive(false);
            }
            else
                Utility.ErrorLog("Loadouts Objects of " + this.gameObject.name + " in LoadoutManager.cs is not assigned", 1);
        }

        if (index < loadouts.Length)
        {
            if (loadouts[index])
            {
                loadouts[index].gameObject.SetActive(true);

                foreach (var item in loadouts[index].weapon)
                {
                    if (item)
                    {
                        item.gameObject.SetActive(true);

                    }
                    else
                        Utility.ErrorLog("Weapons Objects of " + loadouts[index].gameObject.name + " in Loadouts.cs is not assigned", 1);
                }

                int indexEquipped = EncryptedPlayerPrefs.GetInt("Loadout" + index + "Equipped");
                loadouts[index].currentIndex = indexEquipped;

                if (loadouts[index].weapon[indexEquipped].gameObject)
                {
                    loadouts[index].weapon[indexEquipped].gameObject.SetActive(true);
                }
                else
                    Utility.ErrorLog("Weapons Object of " + loadouts[index].gameObject.name + " at index " + indexEquipped + " in Loadouts.cs is not assigned", 1);

            }
            else
                Utility.ErrorLog("Loadouts Object of " + this.gameObject.name + " at index " + index + " in LoadoutManager.cs is not assigned", 1);
        }
        else
            Utility.ErrorLog("Loadout Objects array is out of bound in LoadoutManager.cs", 4);

        Utility.MakeClickSound();
    }
    public void NextWeapon(Loadouts loadout)
    {
        return;
        if (loadout)
        {
            loadout.currentIndex++;
            if (loadout.currentIndex >= loadout.weapon.Length)
            {
                loadout.currentIndex = 1;
            }
            foreach (var item in loadout.weapon)
            {
                if (item)
                {
                    item.gameObject.SetActive(false);
                }
                else
                    Utility.ErrorLog("Weapons Objects of " + loadout.gameObject.name + " in Loadouts.cs is not assigned", 1);
            }
            if (loadout.weapon[loadout.currentIndex])
            {
                loadout.weapon[loadout.currentIndex].SetActive(true);
            }
            else
                Utility.ErrorLog("Weapons Object of " + loadout.gameObject.name + " at index " + loadout.currentIndex + " in Loadouts.cs is not assigned", 1);
        }
        else
            Utility.ErrorLog("Loadout Object in parameter of NextWeapon() in LoadoutManager.cs is not assigned", 1);

        Utility.MakeClickSound();
    }
    public void PreviousWeapon(Loadouts loadout)
    {
        return;
        if (loadout)
        {
            loadout.currentIndex--;
            if (loadout.currentIndex <= 0)
            {
                loadout.currentIndex = loadout.weapon.Length - 1;
            }
            foreach (var item in loadout.weapon)
            {
                if (item)
                {
                    item.gameObject.SetActive(false);
                }
                else
                    Utility.ErrorLog("Weapons Objects of " + loadout.gameObject.name + " in Loadouts.cs is not assigned", 1);
            }
            if (loadout.weapon[loadout.currentIndex])
            {
                loadout.weapon[loadout.currentIndex].SetActive(true);
            }
            else
                Utility.ErrorLog("Weapons Object of " + loadout.gameObject.name + " at index " + loadout.currentIndex + " in Loadouts.cs is not assigned", 1);
        }
        else
            Utility.ErrorLog("Loadout Object in parameter of PreviousWeapon() in LoadoutManager.cs is not assigned", 1);

        Utility.MakeClickSound();
    }

    public void OpenBuyPanel(ItemPurchaseValues decider)
    {
        buyPanel.SetActive(true);
        buyPanel.GetComponent<BuyPanelScript>().price = decider.price;
        buyPanel.GetComponent<BuyPanelScript>().decider = decider;
    }

    public void CloseBuyPanel()
    {
        buyPanel.SetActive(false);
    }

    public void BuyLoadout(ItemPurchaseValues decider)
    {
        int loadoutPrice = decider.price;
        int totalFunds = EncryptedPlayerPrefs.GetInt("Funds");

        if (loadoutPrice <= totalFunds)
        {
            //Utility.MakeClickSound();

            LoadoutLockAndEquipStatus loadoutStatus = decider.gameObject.GetComponentInParent<LoadoutLockAndEquipStatus>() as LoadoutLockAndEquipStatus;
            if (loadoutStatus)
            {
                if (loadoutStatus.prefKey != null)
                {
                    EncryptedPlayerPrefs.SetInt(loadoutStatus.prefKey, 1);
                    EncryptedPlayerPrefs.SetInt(loadoutStatus.prefEquip, loadoutStatus.weaponNumber);

                    if (loadoutStatus.myLoadoutNumberIs == 3)
                    {
                        if (GameServerData.Instance)
                            GameServerData.Instance.SetData("0", "0", loadoutStatus.myWeaponNumberIs);
                    }

                    SetCanvasSprites[] canvasSpritesScripts = FindObjectsOfType<SetCanvasSprites>() as SetCanvasSprites[];
                    foreach (var item in canvasSpritesScripts)
                    {
                        item.Start();
                    }
                    
                    //Debug.Log("loadout pref key is " + loadoutStatus.prefKey + " loadout equip key is " + loadoutStatus.prefEquip +" weapon number is " + loadoutStatus.weaponNumber);
                    if (loadoutStatus.gameObject.GetComponentInParent<Loadouts>())
                    {
                        LoadoutLockAndEquipStatus[] sc = transform.GetComponentsInChildren<LoadoutLockAndEquipStatus>() as LoadoutLockAndEquipStatus[];



                        foreach (var item in sc)
                        {
                            item.OnEnable();
                        }
                        loadoutStatus.gameObject.SetActive(false);
                        StartCoroutine(enable(loadoutStatus.gameObject));
                        //NextWeapon(loadoutStatus.gameObject.GetComponentInParent<Loadouts>());
                        //PreviousWeapon(loadoutStatus.gameObject.GetComponentInParent<Loadouts>());
                    }
                    else
                        Utility.ErrorLog("Loadout Component not found in parent of " + loadoutStatus.gameObject.name, 2);


                    int finalFunds = totalFunds - loadoutPrice;
                    EncryptedPlayerPrefs.SetInt("Funds", finalFunds);

                    Utility.ShowHeaderValues();
                }
            }
            else
                Utility.ErrorLog("Loadout Locked Status Component not found in parent of " + decider.gameObject.name, 2);
        }
        else
        {
            Utility.MakeNotEnoughclickSound();
            if (IngameUI.Instance.InsufficientFundsPanel)
            {
                IngameUI.Instance.InsufficientFundsPanel.SetActive(true);
            }
            else
                Utility.ErrorLog("Insufficient Funds Panel of " + this.gameObject.name + " in LoadoutManager.cs is not assigned", 1);
        }
    }

    IEnumerator enable(GameObject weapon)
    {
        yield return null;
        weapon.SetActive(true);
        EnvironmentManager.Instance.ChangeSky();
    }

    public void EquipWeapon(LoadoutLockAndEquipStatus loadoutStatus)
    {
        if (loadoutStatus)
        {
            if (EncryptedPlayerPrefs.GetInt(loadoutStatus.prefKey) == 1)
            {
                EncryptedPlayerPrefs.SetInt(loadoutStatus.prefEquip, loadoutStatus.weaponNumber);
                
                SetCanvasSprites[] canvasSpritesScripts = FindObjectsOfType<SetCanvasSprites>() as SetCanvasSprites[];
                foreach (var item in canvasSpritesScripts)
                {
                    item.Start();
                }

                if (loadoutStatus.myLoadoutNumberIs == 3)
                {
                    if (GameServerData.Instance)
                        GameServerData.Instance.SetData("0", "0", loadoutStatus.myWeaponNumberIs);
                }

                if (loadoutStatus.gameObject.GetComponentInParent<Loadouts>())
                {
                    loadoutStatus.gameObject.SetActive(false);
                    StartCoroutine(enable(loadoutStatus.gameObject));

                    LoadoutLockAndEquipStatus[] sc = transform.GetComponentsInChildren<LoadoutLockAndEquipStatus>() as LoadoutLockAndEquipStatus[];

                    foreach (var item in sc)
                    {
                        item.OnEnable();
                    }

                    //NextWeapon(loadoutStatus.gameObject.GetComponentInParent<Loadouts>());
                    //PreviousWeapon(loadoutStatus.gameObject.GetComponentInParent<Loadouts>());
                }
                else
                    Utility.ErrorLog("Loadout Component not found in parent of " + loadoutStatus.gameObject.name, 2);
            }
        }
        else
            Utility.ErrorLog("Loadout Status Object in parameter of EquipWeapon() in LoadoutManager.cs is not assigned", 1);
    }
}