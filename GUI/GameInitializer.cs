using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class GameInitializer : MonoBehaviour
{
    public int MaxBTCLimit;
    public int MaxETHLimit;
    public int MaxBNBLimit;
    public int MaxUSDTLimit;
    public int MaxXRPLimit;
    public int MaxDOGELimit;
    public int BTCEarlyLimit;
    public int ETHEarlyLimit;
    public int BNBEarlyLimit;
    public int USDTEarlyLimit;
    public int XRPEarlyLimit;
    public int DOGEEarlyLimit;
    public int BTCEarlyMaxLevels;
    public int ETHEarlyMaxLevels;
    public int BNBEarlyMaxLevels;
    public int USDTEarlyMaxLevels;
    public int XRPEarlyMaxLevels;
    public int DOGEEarlyMaxLevels;

    [Header("Head Positions Instances")]
    public GameObject[] headPositionsInstances;

    public static GameInitializer Instance;


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
        GameManager.Instance.environmentNumber = 0;

        if (GameManager.Instance.environmentType == -1)
        {
            GameManager.Instance.environmentType = Random.Range(0, headPositionsInstances.Length);
        }
        if (GameManager.Instance.levelNumber == 0)
        {
            GameManager.Instance.environmentType = 0;
        }

        foreach (var item in headPositionsInstances)
        {
            item.SetActive(false);
        }
        headPositionsInstances[GameManager.Instance.environmentType].SetActive(true);

        SetCryptoPrefs();
    }
    private void Start()
    {
        int levelNumber = GameManager.Instance.levelNumber;

        if (levelNumber == 0)
        {
            HeadPositionsArray.Instance.headsToPair = 3;
            IngameUI.Instance.numberOfHeadsToPair.text = HeadPositionsArray.Instance.headsToPair.ToString();

            HeadSpawner.Instance.FillInStartWithHeads(levelNumber);


            int totalHeadsToPop = 9999;

            HeadSpawner.Instance.spawnRate = 2;

            if (HeadSpawner.Instance.spawnRate < 5)
            {
                HeadSpawner.Instance.spawnRate = 5;
            }

            TimeController.Instance.SetTimeInMinutes(250);

            IngameUI.Instance.SetNumberOfHeadsToPop(totalHeadsToPop);
        }
        else
        {
            HeadPositionsArray.Instance.headsToPair = /*(levelNumber / 50) + */3;
            IngameUI.Instance.numberOfHeadsToPair.text = HeadPositionsArray.Instance.headsToPair.ToString();

            //int justifiedHeads = HeadSpawner.Instance.GetNumberOfJustifiedHeads();

            //int indexNumber = -1;

            //for (int i = 0; i < HeadSpawner.Instance.numberOfJustifiedHeadsAfterWhichTheBossComes.Length; i++)
            //{
            //    string pref = "BossLevelNumber" + i;

            //    int expectedBossIndex = -1;
            //    if (EncryptedPlayerPrefs.GetInt(pref, 0) == 0)
            //    {
            //        expectedBossIndex = i;
            //    }

            //    if (HeadSpawner.Instance.numberOfJustifiedHeadsAfterWhichTheBossComes[i] == justifiedHeads)
            //    {
            //        indexNumber = i;
            //        break;
            //    }
            //    if (expectedBossIndex != -1)
            //    {
            //        if (justifiedHeads > HeadSpawner.Instance.numberOfJustifiedHeadsAfterWhichTheBossComes[expectedBossIndex])
            //        {
            //            indexNumber = expectedBossIndex;
            //            break;
            //        }
            //    }
            //}


            //bool isBossLevel = false;
            //if (indexNumber != -1)
            //{
            //    string pref = "BossLevelNumber" + indexNumber;

            //    if (EncryptedPlayerPrefs.GetInt(pref, 0) == 0)
            //    {
            //        MoveTowards.bossChangingPosition = false;
            //        isBossLevel = true;
            //        HeadSpawner.Instance.isBossLevel = true;
            //        GameManager.Instance.environmentNumber = 1;
            //        HeadSpawner.Instance.bossIndexNumber = indexNumber;
            //        IngameUI.Instance.TurnBossIntroPanel();


            //        foreach (var item in envBackgrounds)
            //        {
            //            item.SetActive(false);
            //        }
            //        envBackgrounds[GameManager.Instance.environmentNumber].SetActive(true);
            //    }
            //}
            //int justifiedHeads = HeadSpawner.Instance.GetNumberOfJustifiedHeads();

            //int indexNumber = -1;

            //for (int i = 0; i < HeadSpawner.Instance.numberOfJustifiedHeadsAfterWhichTheBossComes.Length; i++)
            //{
            //    string pref = "BossLevelNumber" + i;

            //    int expectedBossIndex = -1;
            //    if (EncryptedPlayerPrefs.GetInt(pref, 0) == 0)
            //    {
            //        expectedBossIndex = i;
            //    }

            //    if (HeadSpawner.Instance.numberOfJustifiedHeadsAfterWhichTheBossComes[i] == justifiedHeads)
            //    {
            //        indexNumber = i;
            //        break;
            //    }
            //    if (expectedBossIndex != -1)
            //    {
            //        if (justifiedHeads > HeadSpawner.Instance.numberOfJustifiedHeadsAfterWhichTheBossComes[expectedBossIndex])
            //        {
            //            indexNumber = expectedBossIndex;
            //            break;
            //        }
            //    }
            //}

            int indexNumber = -1;

            if (levelNumber % 10 == 0)
            {
                indexNumber = (levelNumber / 10) - 1;
            }

            bool isBossLevel = false;
            if (indexNumber != -1)
            {
                string pref = "BossLevelNumber" + indexNumber;

                if (EncryptedPlayerPrefs.GetInt(pref, 0) == 0)
                {
                    MoveTowards.bossChangingPosition = false;
                    isBossLevel = true;
                    HeadSpawner.Instance.isBossLevel = true;
                    GameManager.Instance.environmentNumber = 1;
                    HeadSpawner.Instance.bossIndexNumber = indexNumber;
                    IngameUI.Instance.TurnBossIntroPanel();
                    IngameUI.Instance.bossHeadObjectStartPanel.SetActive(true);
                    foreach (var item in IngameUI.Instance.bossSprites)
                    {
                        item.SetActive(false);
                    }
                    IngameUI.Instance.bossSprites[indexNumber % HeadSpawner.Instance.bossPrefabs.Length].SetActive(true);
                    EnvironmentSelector envSelector = HeadPositionsArray.Instance.gameObject.GetComponent<EnvironmentSelector>();
                    foreach (var item in envSelector.envBackgrounds)
                    {
                        item.SetActive(false);
                    }
                    envSelector.envBackgrounds[GameManager.Instance.environmentNumber].SetActive(true);
                }
            }

            int totalHeadsToPop = ((int)levelNumber) + 9;

            if (isBossLevel)
            {
                totalHeadsToPop *= 1000;
            }
            else
            {
                IngameUI.Instance.bossHeadObjectStartPanel.SetActive(false);
            }

            totalHeadsToPop = totalHeadsToPop * HeadPositionsArray.Instance.headsToPair;

            float spawnRate = HeadSpawner.Instance.spawnRate - (levelNumber * 0.1f);
            if (spawnRate < 5)
            {
                spawnRate = 5;
            }
            HeadSpawner.Instance.SetSpawnRate(spawnRate);

            if (isBossLevel)
            {
                TimeController.Instance.SetTimeInSeconds(((int)((indexNumber + 1) * 50)) * Random.Range(13, 15));
            }
            else
            {
                if (levelNumber <= 10)
                {
                    TimeController.Instance.SetTimeInSeconds(((int)(totalHeadsToPop / 3)) * Random.Range(10, 15));
                }

                else
                {
                    TimeController.Instance.SetTimeInSeconds(((int)(totalHeadsToPop / 3)) * Random.Range(7, 10));
                }
            }

            IngameUI.Instance.SetNumberOfHeadsToPop(totalHeadsToPop);

            HeadSpawner.Instance.FillInStartWithHeads(levelNumber);
        }
    }
    void SetCryptoPrefs()
    {
        int numberOfLevelsToGiveEarlyCoins = 200;


        if (numberOfLevelsToGiveEarlyCoins != EncryptedPlayerPrefs.GetInt("NumberOfLevelsDistributionForPrizes"))
        {
            EncryptedPlayerPrefs.SetInt("NumberOfLevelsDistributionForPrizes", numberOfLevelsToGiveEarlyCoins);

            float BTCDistributionPerLevel = (float)BTCEarlyLimit / (float)BTCEarlyMaxLevels;
            float ETHDistributionPerLevel = (float)ETHEarlyLimit / (float)ETHEarlyMaxLevels;
            float BNBDistributionPerLevel = (float)BNBEarlyLimit / (float)BNBEarlyMaxLevels;
            float USDTDistributionPerLevel = (float)USDTEarlyLimit / (float)USDTEarlyMaxLevels;
            float XRPDistributionPerLevel = (float)XRPEarlyLimit / (float)XRPEarlyMaxLevels;
            float DOGEDistributionPerLevel = (float)DOGEEarlyLimit / (float)DOGEEarlyMaxLevels;

            //Debug.Log(BTCDistributionPerLevel);
            //Debug.Log(ETHDistributionPerLevel);
            //Debug.Log(BNBDistributionPerLevel);
            //Debug.Log(USDTDistributionPerLevel);
            //Debug.Log(XRPDistributionPerLevel);
            //Debug.Log(DOGEDistributionPerLevel);


            int levelsDistributionOfBTC = Mathf.RoundToInt(1 / BTCDistributionPerLevel);
            int levelsDistributionOfETH = Mathf.RoundToInt(1 / ETHDistributionPerLevel);
            int levelsDistributionOfBNB = Mathf.RoundToInt(1 / BNBDistributionPerLevel);
            int levelsDistributionOfUSDT = Mathf.RoundToInt(1 / USDTDistributionPerLevel);
            int levelsDistributionOfXRP = Mathf.RoundToInt(1 / XRPDistributionPerLevel);
            int levelsDistributionOfDOGE = Mathf.RoundToInt(1 / DOGEDistributionPerLevel);


            //Debug.Log(levelsDistributionOfBTC);
            //Debug.Log(levelsDistributionOfETH);
            //Debug.Log(levelsDistributionOfBNB);
            //Debug.Log(levelsDistributionOfUSDT);
            //Debug.Log(levelsDistributionOfXRP);
            //Debug.Log(levelsDistributionOfDOGE);

            int maxNumberOfLevels = 1000;
            int returningIndex;
            int remaining;
            int factor;

            SetStringInPref("BTCinLevels", maxNumberOfLevels);
            returningIndex = Distribute("BTCinLevels", BTCDistributionPerLevel, levelsDistributionOfBTC, 0, BTCEarlyMaxLevels);

            remaining = MaxBTCLimit - BTCEarlyLimit;
            factor = (maxNumberOfLevels - returningIndex) / remaining;
            if (factor == 0) factor = 1;
            DistributeRemaining("BTCinLevels", factor, returningIndex, remaining);


            SetStringInPref("ETHinLevels", maxNumberOfLevels);
            returningIndex = Distribute("ETHinLevels", ETHDistributionPerLevel, levelsDistributionOfETH, 0, ETHEarlyMaxLevels);

            remaining = MaxETHLimit - ETHEarlyLimit;
            factor = (maxNumberOfLevels - returningIndex) / remaining;
            if (factor == 0) factor = 1;
            DistributeRemaining("ETHinLevels", factor, returningIndex, remaining);


            SetStringInPref("BNBinLevels", maxNumberOfLevels);
            returningIndex = Distribute("BNBinLevels", BNBDistributionPerLevel, levelsDistributionOfBNB, 0, BNBEarlyMaxLevels);

            remaining = MaxBNBLimit - BNBEarlyLimit;
            factor = (maxNumberOfLevels - returningIndex) / remaining;
            if (factor == 0) factor = 1;
            DistributeRemaining("BNBinLevels", factor, returningIndex, remaining);


            SetStringInPref("USDTinLevels", maxNumberOfLevels);
            returningIndex = Distribute("USDTinLevels", USDTDistributionPerLevel, levelsDistributionOfUSDT, 0, USDTEarlyMaxLevels);

            remaining = MaxUSDTLimit - USDTEarlyLimit;
            factor = (maxNumberOfLevels - returningIndex) / remaining;
            if (factor == 0) factor = 1;
            DistributeRemaining("USDTinLevels", factor, returningIndex, remaining);


            SetStringInPref("XRPinLevels", maxNumberOfLevels);
            returningIndex = Distribute("XRPinLevels", XRPDistributionPerLevel, levelsDistributionOfXRP, 0, XRPEarlyMaxLevels);

            remaining = MaxXRPLimit - XRPEarlyLimit;
            factor = (maxNumberOfLevels - returningIndex) / remaining;
            if (factor == 0) factor = 1;
            DistributeRemaining("XRPinLevels", factor, returningIndex, remaining);


            SetStringInPref("DOGEinLevels", maxNumberOfLevels);
            returningIndex = Distribute("DOGEinLevels", DOGEDistributionPerLevel, levelsDistributionOfDOGE, 0, DOGEEarlyMaxLevels);

            remaining = MaxDOGELimit - DOGEEarlyLimit;
            factor = (maxNumberOfLevels - returningIndex) / remaining;
            if (factor == 0) factor = 1;
            DistributeRemaining("DOGEinLevels", factor, returningIndex, remaining);


            //int count = 0;
            //string s = EncryptedPlayerPrefs.GetString("DOGEinLevels");
            //for (int i = returningIndex; i < s.Length; i++)
            //{
            //    if (s[i] != ',')
            //    {
            //        count += int.Parse(s[i].ToString());
            //    }
            //}
        }
        //Debug.Log(EncryptedPlayerPrefs.GetString("BTCinLevels"));
        //Debug.Log(EncryptedPlayerPrefs.GetString("ETHinLevels"));
        //Debug.Log(EncryptedPlayerPrefs.GetString("BNBinLevels"));
        //Debug.Log(EncryptedPlayerPrefs.GetString("USDTinLevels"));
        //Debug.Log(EncryptedPlayerPrefs.GetString("XRPinLevels"));
        //Debug.Log(EncryptedPlayerPrefs.GetString("DOGEinLevels"));


        //int count = 0;
        //string s = EncryptedPlayerPrefs.GetString("XRPinLevels");
        //for (int i = 0; i < s.Length; i++)
        //{
        //    if (s[i] != ',')
        //    {
        //        count += int.Parse(s[i].ToString());
        //    }
        //}
        //Debug.Log(count);
    }
    void SetStringInPref(string prefKey, int totalNumberOfLevels)
    {
        string temp = "";
        for (int i = 0; i < totalNumberOfLevels; i++)
        {
            temp = temp + "0,";
        }
        EncryptedPlayerPrefs.SetString(prefKey, temp);
    }
    int Distribute(string prefKey, float DistributionPerLevel, int levelsDistribution, int initialValue, int finalValue)
    {
        string prefValue = EncryptedPlayerPrefs.GetString(prefKey);

        int i = 0;

        if (DistributionPerLevel < 1)
        {
            //multiply by 2 because of the ',' in the string index

            int distribute = levelsDistribution * 2;

            for (i = initialValue; i < finalValue * 2; i++)
            {
                if (prefValue[i] != ',')
                {
                    if (i % distribute == 0)
                    {
                        int randomMultiplier = Random.Range(0, 3);

                        int index = i + (randomMultiplier * 2);
                        if (index < prefValue.Length)
                        {
                            int value = int.Parse(prefValue[index].ToString());
                            value = value + 1;
                            prefValue = prefValue.Remove(index, 1);
                            prefValue = prefValue.Insert(index, value.ToString());
                        }
                        i = i + distribute;
                        i--;
                    }
                }
            }
        }
        else if (DistributionPerLevel >= 1)
        {
            int count = 0;
            for (i = initialValue; i < finalValue * 2; i++)
            {
                if (prefValue[i] != ',')
                {
                    count++;
                    int index = i;
                    int value = Mathf.RoundToInt(DistributionPerLevel);
                    prefValue = prefValue.Remove(index, 1);
                    prefValue = prefValue.Insert(index, value.ToString());
                }
            }
        }
        EncryptedPlayerPrefs.SetString(prefKey, prefValue);

        return i;
    }
    void DistributeRemaining(string prefKey, int levelsDistribution, int initialValue, int totalToDistribute)
    {
        string prefValue = EncryptedPlayerPrefs.GetString(prefKey);

        int distribute = levelsDistribution * 2;
        if (prefValue[initialValue] == ',')
        {
            initialValue++;
        }

        for (int i = 0; i < totalToDistribute; i++)
        {
            int value = int.Parse(prefValue[initialValue].ToString());
            value = value + 1;
            prefValue = prefValue.Remove(initialValue, 1);
            prefValue = prefValue.Insert(initialValue, value.ToString());

            initialValue = initialValue + distribute;
        }
        EncryptedPlayerPrefs.SetString(prefKey, prefValue);
    }
}