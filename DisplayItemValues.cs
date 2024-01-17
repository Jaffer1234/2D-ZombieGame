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
        Highscore
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
                itemText.text = EncryptedPlayerPrefs.GetString("PlayerName");
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
                    itemText.text = EncryptedPlayerPrefs.GetString("PlayerName");
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
}
