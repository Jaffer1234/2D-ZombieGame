using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayItemValues : MonoBehaviour
{
    public enum Items
    {
        Funds,
        LineBomb,
        RadiusBomb,
        TimeBomb,
        playerName,
        LevelNum,
        Highscore,
        BTC,
        ETH,
        BNB,
        USDT,
        XRP,
        DOGE,
    }

    [Header("Select Item")]
    public Items item;
    [Header("Item Text")]
    public UnityEngine.UI.Text itemText;

    
    void OnEnable()
    {
        ShowCount();    
    }
    public void ShowCount()
    {
        if (itemText)
        {
            if (item == Items.Funds)
            {
                itemText.text = EncryptedPlayerPrefs.GetInt("Funds").ToString();
            }
            else if (item == Items.LineBomb)
            {
                itemText.text = EncryptedPlayerPrefs.GetInt("LineBomb").ToString();
            }
            else if (item == Items.RadiusBomb)
            {
                itemText.text = EncryptedPlayerPrefs.GetInt("RadiusBomb").ToString();
            }
            else if (item == Items.TimeBomb)
            {
                itemText.text = EncryptedPlayerPrefs.GetInt("TimeBomb").ToString();
            }
            else if (item == Items.playerName)
            {
                //if (EncryptedPlayerPrefs.GetInt("playerNameSet") == 0)
                //{
                //    itemText.text = EncryptedPlayerPrefs.GetString("PlayerGuestName").ToString();
                //}
                //else
                {
                    itemText.text = EncryptedPlayerPrefs.GetString("PlayerName").ToString();
                }
            }
            else if (item == Items.LevelNum)
            {
                itemText.text = EncryptedPlayerPrefs.GetInt("LevelsUnocked").ToString();
            }
            else if (item == Items.Highscore)
            {
                itemText.text = EncryptedPlayerPrefs.GetString("Highscore").ToString();
            }
        }
        else
        {
            if (this.gameObject.GetComponentInChildren<UnityEngine.UI.Text>())
            {
                itemText = this.gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
                if (item == Items.Funds)
                {
                    itemText.text = EncryptedPlayerPrefs.GetInt("Funds").ToString();
                }
                else if (item == Items.LineBomb)
                {
                    itemText.text = EncryptedPlayerPrefs.GetInt("LineBomb").ToString();
                }
                else if (item == Items.RadiusBomb)
                {
                    itemText.text = EncryptedPlayerPrefs.GetInt("RadiusBomb").ToString();
                }
                else if (item == Items.playerName)
                {
                    if (EncryptedPlayerPrefs.GetInt("playerNameSet") == 0)
                    {
                        itemText.text = EncryptedPlayerPrefs.GetString("PlayerGuestName").ToString();
                    }
                    else
                    {
                        itemText.text = EncryptedPlayerPrefs.GetString("PlayerName").ToString();
                    }
                }
                else if (item == Items.LevelNum)
                {
                    itemText.text = EncryptedPlayerPrefs.GetInt("LevelsUnocked").ToString();
                }
                else if (item == Items.Highscore)
                {
                    itemText.text = EncryptedPlayerPrefs.GetString("Highscore").ToString();
                }
            }
            else
                Utility.ErrorLog("Text is not assigned of " + this.gameObject.name + " in DisplayItemValues.cs", 1);
        }
    }

    public void ShowCryptoValues()
    {
        if (!itemText)
        {
            if (this.gameObject.GetComponent<UnityEngine.UI.Text>())
            {
                itemText = this.gameObject.GetComponent<UnityEngine.UI.Text>();
                ShowCount();
            }
        }

        if (item == Items.BTC)
        {
            itemText.text = EncryptedPlayerPrefs.GetInt("BTCCollected").ToString();
        }
        else
        if (item == Items.ETH)
        {
            itemText.text = EncryptedPlayerPrefs.GetInt("ETHCollected").ToString();
        }
        else
        if (item == Items.BNB)
        {
            itemText.text = EncryptedPlayerPrefs.GetInt("BNBCollected").ToString();
        }
        else
        if (item == Items.USDT)
        {
            itemText.text = EncryptedPlayerPrefs.GetInt("USDTCollected").ToString();
        }
        else
        if (item == Items.XRP)
        {
            itemText.text = EncryptedPlayerPrefs.GetInt("XRPCollected").ToString();
        }
        else
        if (item == Items.DOGE)
        {
            itemText.text = EncryptedPlayerPrefs.GetInt("DOGECollected").ToString();
        }
    }
}
