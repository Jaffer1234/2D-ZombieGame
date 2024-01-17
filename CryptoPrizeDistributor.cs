using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CryptoPrizeDistributor : MonoBehaviour
{
    public GameObject target;
    public GameObject[] cryptPrefabs;

    private int totalPrizesInALevel = 0;
    private float totalDistributionFactor = 0f;

    private float totalIntervalsPassed = 0f;

    public static CryptoPrizeDistributor Instance;

    //[HideInInspector]
    public List<int> prizesToDistributeList;

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        prizesToDistributeList = new List<int>();
    }

    private void Start()
    {
        CalculateNumberOfPrizes();
    }
    public void FillForTutorial()
    {
        prizesToDistributeList.Add(0);
        prizesToDistributeList.Add(0);
        prizesToDistributeList.Add(1);
        prizesToDistributeList.Add(1);
        prizesToDistributeList.Add(4);
        prizesToDistributeList.Add(4);
        prizesToDistributeList.Add(5);
        prizesToDistributeList.Add(5);
        totalPrizesInALevel = prizesToDistributeList.Count;
        totalDistributionFactor = 1 / (float)totalPrizesInALevel;
        totalIntervalsPassed = 0 - (totalDistributionFactor / 2);
    }
    public int GetTotalToSpawn(float progress)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return 0;
        }
        if ((totalIntervalsPassed + totalDistributionFactor) < progress)
        {
            float difference = progress - totalIntervalsPassed;
            int totalToSpawn = Mathf.RoundToInt((difference / totalDistributionFactor));
            totalIntervalsPassed += (float)totalToSpawn * totalDistributionFactor;
            return totalToSpawn;
        }
        else
        {
            return 0;
        }
    }
    public void SpawnPrizes(int number, Vector3 spawnPosition)
    {
        return;
        for(int i = 0; i < number; i++)
        {
            int randomCurrency = Random.Range(0, prizesToDistributeList.Count);

            if (randomCurrency >= prizesToDistributeList.Count)
            {
                return;
            }

            // tell head spawner
            int id = prizesToDistributeList[randomCurrency];

            GameObject crypto = Instantiate(cryptPrefabs[id], spawnPosition, Quaternion.identity);
            crypto.GetComponent<PrizeAnimate>().AssignIndex(id, false, target.transform);
            SoundManager.Instance.PlayCryptoCoin();
            IngameUI.Instance.UpdateCryptoValues(id);
            SetPref(id);
            prizesToDistributeList.RemoveAt(randomCurrency);
        }
    }
    public void CalculateNumberOfPrizes()
    {
        if ((GameManager.Instance.levelNumber - 1) >= 0)
        {
            int levelIndex = GameManager.Instance.levelNumber - 1;

            levelIndex = levelIndex * 2;

            string prefValueBTC = EncryptedPlayerPrefs.GetString("BTCinLevels");
            string prefValueETH = EncryptedPlayerPrefs.GetString("ETHinLevels");
            string prefValueBNB = EncryptedPlayerPrefs.GetString("BNBinLevels");
            string prefValueUSDT = EncryptedPlayerPrefs.GetString("USDTinLevels");
            string prefValueXRP = EncryptedPlayerPrefs.GetString("XRPinLevels");
            string prefValueDOGE = EncryptedPlayerPrefs.GetString("DOGEinLevels");

            int count;
            if (levelIndex < prefValueBTC.Length)
            {
                count = int.Parse(prefValueBTC[levelIndex].ToString());
                for (int i = 0; i < count; i++)
                {
                    prizesToDistributeList.Add(0);
                }
            }
            if (levelIndex < prefValueETH.Length)
            {
                count = int.Parse(prefValueETH[levelIndex].ToString());
                for (int i = 0; i < count; i++)
                {
                    prizesToDistributeList.Add(1);
                }
            }
            if (levelIndex < prefValueBNB.Length)
            {
                count = int.Parse(prefValueBNB[levelIndex].ToString());
                for (int i = 0; i < count; i++)
                {
                    prizesToDistributeList.Add(2);
                }
            }
            if (levelIndex < prefValueUSDT.Length)
            {
                count = int.Parse(prefValueUSDT[levelIndex].ToString());
                for (int i = 0; i < count; i++)
                {
                    prizesToDistributeList.Add(3);
                }
            }
            if (levelIndex < prefValueXRP.Length)
            {
                count = int.Parse(prefValueXRP[levelIndex].ToString());
                for (int i = 0; i < count; i++)
                {
                    prizesToDistributeList.Add(4);
                }
            }
            if (levelIndex < prefValueDOGE.Length)
            {
                count = int.Parse(prefValueDOGE[levelIndex].ToString());
                for (int i = 0; i < count; i++)
                {
                    prizesToDistributeList.Add(5);
                }
            }
            totalPrizesInALevel = prizesToDistributeList.Count;
            totalDistributionFactor = 1 / (float)totalPrizesInALevel;
            totalIntervalsPassed = 0 - (totalDistributionFactor / 2);
        }
    }
    void SetPref(int id)
    {
        if (id == 0) SetPrefInLevels("BTCinLevels");
        else if (id == 1) SetPrefInLevels("ETHinLevels");
        else if (id == 2) SetPrefInLevels("BNBinLevels");
        else if (id == 3) SetPrefInLevels("USDTinLevels");
        else if (id == 4) SetPrefInLevels("XRPinLevels");
        else if (id == 5) SetPrefInLevels("DOGEinLevels");
    }
    void SetPrefInLevels(string prefKey)
    {
        if (GameManager.Instance.levelNumber == 0) return;

        string prefValue = EncryptedPlayerPrefs.GetString(prefKey);

        int levelIndex = GameManager.Instance.levelNumber - 1;

        levelIndex = levelIndex * 2;

        int count = int.Parse(prefValue[levelIndex].ToString());

        count--;

        if (count <= 0)
        {
            count = 0;
        }

        prefValue = prefValue.Remove(levelIndex, 1);
        prefValue = prefValue.Insert(levelIndex, count.ToString());

        EncryptedPlayerPrefs.SetString(prefKey, prefValue);
    }
}
