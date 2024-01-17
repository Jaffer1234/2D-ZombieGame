using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class IngameUI : MonoBehaviour
{
    public int levelsAfterWhichFallingHeadIsIntroduced = 2;
    public int levelsAfterWhichDevilHeadIsIntroduced = 4;
    public int levelsAfterWhichIdGetterIsIntroduced = 4;
    public int levelsAfterWhichCopyHeadIsIntroduced = 4;
    public int levelsAfterWhichANewHeadIsSpawned = 5;
    public int levelsAfterWhichAFreezingHeadIsIntroduced = 8;

    public HeadsCountKeeper headCounterScript;
    public GameObject bossHeadObjectStartPanel;
    public GameObject[] bossSprites;

    [Header("Sound And Music")]
    public SoundManager soundSource;
    public MusicManager musicSource;
    public static SoundManager _soundSource;
    public static MusicManager _musicSource;

    public static bool IsPlayingIngame = false;
    public static bool isPlaying = false;
    
    [Header("Public Menu Panels")]
    public GameObject StartPanel;
    public GameObject ingamePanel;
    public GameObject pausePanel;
    public GameObject failPanel;
    public GameObject successPanel;
    public GameObject ReSpawnPanel;
    public GameObject itemShopPanel;
    public GameObject InsufficientFundsPanel;
    public GameObject loadingPanel;
    public GameObject RewardPanel;
    public GameObject rewardNotAvailable;
    public GameObject rewardLost;
    public GameObject tutorialPanel;
    public GameObject giveCoinsAnimation;
    public GameObject fourXAnimation;
    public GameObject newHeadsPanel;
    public GameObject bossIntroPanel;
    public GameObject devilHeadIntroductionPanel;
    public GameObject fallingHeadIntroductionPanel;
    public GameObject idGetterHeadIntroductionPanel;
    public GameObject copyHeadIntroductionPanel;
    public GameObject freezingHeadIntroductionPanel;
    public GameObject justifyHeadPanel;
    public GameObject justifiedHeadsRewardsPanel;
    public GameObject TimeLeftCoinsAnim;
    public GameObject BounsLevelPanel;
    public GameObject SuccessCryptoPanel;
    public GameObject FailCryptoPanel;

    [Header("Public Buttons")]
    public Button lineBombBtn;
    public Button radiusBombBtn;
    public Button timeBtn;

    [Header("Sprite Masks")]
    public SpriteMask[] pipeSpriteMasks;

    [Header("Public Text Fields")]
    public Text lineBombText;
    public Text radiusBombText;
    public Text timeBombText;
    public Text levelNumberText;
    public Text LevelTextText;
    public Text ingameLevelText;
    public Text timerText;
    public Text numberOfHeadsToPair;
    public Text numberOfHeadsPoppedOverToPop;
    public Text headsToDestroyStartPanel;
    public Text timeAvailableStartPanel;
    public Text rewardText;
    public Text fundsEarnedSuccessText;
    public Text fundsEarnedFailText;
    public Text fundsEarnedIngameText;
    public Text headsPoppedSuccessText;
    public Text headsPoppedFailText;
    public Text timeTakenSuccessText;
    public Text timeTakenFailText;
    public Text gameScoreText;

    [Header("Public Image Fields")]
    public Image timeFillBar;
    public Image lineBombImage;
    public Image radiusBombImage;
    public GameObject bg_WallLight;
    public GameObject bg_PipesLight;

    [Header("Vibration Sprites")]
    [SerializeField] private Sprite vibrationOn;
    [SerializeField] private Sprite vibrationOff;
    [SerializeField] private Image vibrationObj;

    public float musicLowValue = 0.25f;

    public static IngameUI Instance;

    private int totalFundsEarned;
    private int numberOfHeadsPopped;
    private int numberOfHeadsToPop;
    private int lineBombCount = 0;
    private int radiusBombCount = 0;
    private int timeBombCount = 0;
    private ulong gameScore = 0;

    private int collectionOfBTC;
    public int CollectionOfBTC { get => collectionOfBTC; set => collectionOfBTC = value; }

    private int collectionOfETH;
    public int CollectionOfETH { get => collectionOfETH; set => collectionOfETH = value; }

    private int collectionOfBNB;
    public int CollectionOfBNB { get => collectionOfBNB; set => collectionOfBNB = value; }

    private int collectionOfUSDT;
    public int CollectionOfUSDT { get => collectionOfUSDT; set => collectionOfUSDT = value; }

    private int collectionOfXRP;
    public int CollectionOfXRP { get => collectionOfXRP; set => collectionOfXRP = value; }

    private int collectionOfDOGE;
    public int CollectionOfDOGE { get => collectionOfDOGE; set => collectionOfDOGE = value; }
    

    private int BTCCollectedStart; public int BTCCollectedAtStart { get => BTCCollectedStart;}
    private int ETHCollectedStart; public int ETHCollectedAtStart { get => ETHCollectedStart; }
    private int BNBCollectedStart; public int BNBCollectedAtStart { get => BNBCollectedStart; }
    private int USDTCollectedStart; public int USDTCollectedAtStart { get => USDTCollectedStart; }
    private int XRPCollectedStart; public int XRPCollectedAtStart { get => XRPCollectedStart; }
    private int DOGECollectedStart; public int DOGECollectedAtStart { get => DOGECollectedStart; }

    private ItemController itemController;
    private GameObject panelToOpenNext = null;



    public delegate void ApplicationOutOfFocusOperations();
    public static ApplicationOutOfFocusOperations applicationUnfocus;
    public delegate void ApplicationInFocusOperations();
    public static ApplicationInFocusOperations applicationfocus;

    private bool isOutOfFocus = false;
    private bool hasDataBeenUpdated = false;
    private int totalFundsAtStart = 0;

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
        _soundSource = soundSource;
        _musicSource = musicSource;
        PlayMusic();
    }

    void Start ()
    {
        Time.timeScale = 0.0f;

        ChangeVibrationSprite();

        if (StartPanel)
        {
            StartPanel.SetActive(true);
            SoundManager.Instance.PlayTaptoplaySound();
        }
        else
            Utility.ErrorLog("Start Panel is not assigned in IngameUI.cs", 1);

        if (ItemController.Instance)
        {
            itemController = ItemController.Instance;
        }
        else
            Utility.ErrorLog("ItemController could not be found in IngameUI.cs of " + this.gameObject, 1);

        GameManager.Instance.eyesPopped = 0;
        

        totalFundsAtStart = EncryptedPlayerPrefs.GetInt("Funds");
        fundsEarnedIngameText.text = totalFundsAtStart.ToString();

        BTCCollectedStart = EncryptedPlayerPrefs.GetInt("BTCCollected");
        ETHCollectedStart = EncryptedPlayerPrefs.GetInt("ETHCollected");
        BNBCollectedStart = EncryptedPlayerPrefs.GetInt("BNBCollected");
        USDTCollectedStart = EncryptedPlayerPrefs.GetInt("USDTCollected");
        XRPCollectedStart = EncryptedPlayerPrefs.GetInt("XRPCollected");
        DOGECollectedStart = EncryptedPlayerPrefs.GetInt("DOGECollected");

        collectionOfBTC = BTCCollectedStart;
        collectionOfETH = ETHCollectedStart;
        collectionOfBNB = BNBCollectedStart;
        collectionOfUSDT = USDTCollectedStart;
        collectionOfXRP = XRPCollectedStart;
        collectionOfDOGE = DOGECollectedStart;


        if (levelNumberText)
        {
            levelNumberText.text = GameManager.Instance.levelNumber.ToString();
        }
        else
            Utility.ErrorLog("Level Number Text is not assigned in IngameUI.cs of " + this.gameObject, 1);

        isPlaying = false;
        IsPlayingIngame = false;
        GameIsOutOfIngameOperations(true);
        totalFundsEarned = 0;
        numberOfHeadsPopped = 0;
        MoveTowards.speedMultiplier = 1;
        UpdateScore(0, null);

        if (HeadSpawner.Instance.isBonusLevel)
        {
            lineBombBtn.interactable = false;
            radiusBombBtn.interactable = false;
            timeBtn.interactable = false;
        }
        else
        {
            lineBombBtn.interactable = true;
            radiusBombBtn.interactable = true;
            timeBtn.interactable = true;
        }

        ReSpawnPanel.SetActive(false);

        foreach (var item in pipeSpriteMasks)
        {
            item.enabled = false;
        }
        AdManager.Instance.ShowInterstitialAd();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            //if (ManageNotifications.Instance)
            //{
            //    ManageNotifications.Instance.CancelNotifications();
            //}
            if (applicationfocus != null)
            {
                applicationfocus();
            }
            //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            //{
            //    if (Share.shareStarted)
            //    {
            //        Share.shareStarted = false;
            //        CheckForShareRewards();
            //    }
            //}
            if (IsPlayingIngame)
            {
                if (isOutOfFocus)
                {
                    isOutOfFocus = false;
                }
            }
        }
        else
        {
            //if (ManageNotifications.Instance)
            //{
            //    ManageNotifications.Instance.ScheduleNotifications();
            //}
            if (applicationUnfocus != null)
            {
                applicationUnfocus();
            }
            if (IsPlayingIngame)
            {
                if (!isOutOfFocus)
                {
                    isOutOfFocus = true;
                    UpdateDataOfGame();
                }
            }
        }
    }
    public void UpdateDataOfGame()
    {
        if (hasDataBeenUpdated == false)
        {
            EncryptedPlayerPrefs.SetInt("Funds", totalFundsAtStart);
            EncryptedPlayerPrefs.SetInt("LineBomb", lineBombCount);
            EncryptedPlayerPrefs.SetInt("RadiusBomb", radiusBombCount);
            EncryptedPlayerPrefs.SetInt("TimeBomb", timeBombCount);

            EncryptedPlayerPrefs.SetInt("BTCCollected", collectionOfBTC);
            EncryptedPlayerPrefs.SetInt("ETHCollected", collectionOfETH);
            EncryptedPlayerPrefs.SetInt("BNBCollected", collectionOfBNB);
            EncryptedPlayerPrefs.SetInt("USDTCollected", collectionOfUSDT);
            EncryptedPlayerPrefs.SetInt("XRPCollected", collectionOfXRP);
            EncryptedPlayerPrefs.SetInt("DOGECollected", collectionOfDOGE);
        }
    }
    public void UpdateCryptoValues(int id)
    {
        if (id == 0) collectionOfBTC++;
        else if (id == 1) collectionOfETH++;
        else if (id == 2) collectionOfBNB++;
        else if (id == 3) collectionOfUSDT++;
        else if (id == 4) collectionOfXRP++;
        else if (id == 5) collectionOfDOGE++;
    }
    public void playLightningEffect()
    {
        bg_WallLight.GetComponent<Animator>().SetTrigger("light");
        bg_PipesLight.GetComponent<Animator>().SetTrigger("light");
    }

    public void TurnBossIntroPanel()
    {
        if (!EncryptedPlayerPrefs.HasKey("BossIntroPanel"))
        {
            EncryptedPlayerPrefs.SetInt("BossIntroPanel", 1);
            bossIntroPanel.SetActive(true);
        }
    }

    public void SetNumberOfHeadsToPop(int number)
    {
        headsToDestroyStartPanel.text = number.ToString();
        numberOfHeadsToPop = number;
        numberOfHeadsPoppedOverToPop.text = numberOfHeadsPopped.ToString() + "/" + numberOfHeadsToPop.ToString();
        
        if (GameManager.Instance.levelNumber == 0 || HeadSpawner.Instance.isBossLevel)
        {
            if (!HeadSpawner.Instance.isBossLevel)
            {
                timerText.text = "∞";
                timeAvailableStartPanel.text = "∞";
                headsToDestroyStartPanel.text = "∞";
                LevelTextText.text = "TUTORIAL";
                LevelTextText.text = LocalizationManager.GetTermTranslation("TUTORIAL");
            }
            else
            {
                //LevelTextText.text = "BOSS LEVEL";
                LevelTextText.text = LocalizationManager.GetTermTranslation("BOSS LEVEL");
                //headsToDestroyStartPanel.text = "BOSS HEAD";
                headsToDestroyStartPanel.text = LocalizationManager.GetTermTranslation("BOSS HEAD");
            }
            numberOfHeadsPoppedOverToPop.text = "∞";
            levelNumberText.gameObject.SetActive(false);
        }
    }

    bool firstSpawning = false;
    bool secondSpawning = false;


    public void UpdateNumberOfHeadsPopped(bool increaseProgress, Vector3 location)
    {
        if (increaseProgress)
        {
            numberOfHeadsPoppedOverToPop.gameObject.GetComponent<Animator>().SetTrigger("add");
        }

        int levelNo = GameManager.Instance.levelNumber;
        levelNo = levelNo / 2;
        if (levelNo == 0)
            levelNo = 1;
       
        int min = (int)((levelNo / 2) * 2);
        int max = (int)((levelNo / 2) * 3);
        min = Mathf.Clamp(min, 1, min);
        max = Mathf.Clamp(max, 1, max);

        if ((levelNo * 2) < 5)
        {
            min += 1;
            max += 2;
        }

        int earned = Random.Range(min, max);
        //int totalFunds = EncryptedPlayerPrefs.GetInt("Funds");
        //totalFunds = totalFunds + earned;
        //EncryptedPlayerPrefs.SetInt("Funds", totalFunds);
        earned = Mathf.Clamp(earned, 1, 5);
        totalFundsAtStart += earned;
        fundsEarnedIngameText.text = totalFundsAtStart.ToString();

        totalFundsEarned = totalFundsEarned + earned;

        if (increaseProgress)
        {
            numberOfHeadsPopped++;
            numberOfHeadsPoppedOverToPop.text = numberOfHeadsPopped.ToString() + "/" + numberOfHeadsToPop.ToString();
            CryptoPrizeDistributor.Instance.SpawnPrizes(CryptoPrizeDistributor.Instance.GetTotalToSpawn((float)numberOfHeadsPopped / (float)numberOfHeadsToPop), location);
        }
        if (GameManager.Instance.levelNumber >= levelsAfterWhichFallingHeadIsIntroduced)
        {
            float progress = (float)numberOfHeadsPopped / (float)numberOfHeadsToPop;
            if (progress > 0.2f)
            {
                if (!firstSpawning)
                {
                    FallingObjectSpawner.Instance.SpawnFallingObject();
                    firstSpawning = true;
                }
            }
            else
            if (progress > 0.3f)
            {
                if (!firstSpawning)
                {
                    FallingObjectSpawner.Instance.SpawnFallingObject();
                    firstSpawning = true;
                }
            }

            if (progress > 0.8f)
            {
                if (!firstSpawning)
                {
                    FallingObjectSpawner.Instance.SpawnFallingObject();
                    firstSpawning = true;
                }
            }
            else 
            if (progress > 0.4f)
            {
                if (!secondSpawning)
                {
                    FallingObjectSpawner.Instance.SpawnFallingObject();
                    secondSpawning = true;
                }
            }
        }

        if (numberOfHeadsPopped >= numberOfHeadsToPop)
        {
            GameManager.Instance.GameWon();
        }

        if (GameManager.Instance.levelNumber == 0)
        {
            numberOfHeadsPoppedOverToPop.text = "∞";
        }
    }

    public void AddScoreOfHeadsBlast(int totalBlasted, GameObject scoreObject)
    {
        int score = totalBlasted * 20;
        for (int i = 0; i < totalBlasted; i++)
        {
            if (i < HeadPositionsArray.Instance.headsToBlast.Count)
            {
                int idOfHead = HeadPositionsArray.Instance.headsToBlast[i].GetComponent<HeadScript>().Id;
                if (idOfHead < 100 || (idOfHead >= 1000 && idOfHead < 10000))
                {
                    score = score + (idOfHead * 25);
                }
                else
                if ((idOfHead >= 1000 && idOfHead < 10000))
                {
                    score = score + idOfHead;
                }
                else
                {
                    score = score + (100 * 25);
                }
            }
        }
        UpdateScore(score, scoreObject);
    }

    public void UpdateScore(int scoreToAdd, GameObject scoreObject)
    {
        if (scoreObject)
        {
            scoreObject.GetComponentInChildren<TextMeshPro>().text = "+" + scoreToAdd.ToString();
        }

        if (gameScoreText)
        {
            gameScoreText.gameObject.GetComponent<Animator>().SetTrigger("add");
            gameScore += (ulong)scoreToAdd;
            gameScoreText.text = gameScore.ToString();
        }
    }
    public void UpdateItemValuesCount()
    {
        lineBombCount = EncryptedPlayerPrefs.GetInt("LineBomb");
        if (lineBombText)
            lineBombText.text = lineBombCount.ToString();
        else
            Utility.ErrorLog("Line Bomb Text is not assigned in IngameUI.cs of " + this.gameObject, 1);
        
        radiusBombCount = EncryptedPlayerPrefs.GetInt("RadiusBomb");
        if (radiusBombText)
            radiusBombText.text = radiusBombCount.ToString();
        else
            Utility.ErrorLog("Radius Bomb Text is not assigned in IngameUI.cs of " + this.gameObject, 1);

        timeBombCount = EncryptedPlayerPrefs.GetInt("TimeBomb");
        if (timeBombText)
            timeBombText.text = timeBombCount.ToString();
        else
            Utility.ErrorLog("Time Bomb Text is not assigned in IngameUI.cs of " + this.gameObject, 1);
    }

    public void UseLineBomb()
    {
        if (TutorialManager.isTutorialRunning)
        {
            if (EncryptedPlayerPrefs.GetInt("Tutorial") == 3)
            {
                if (TutorialManager.Instance)
                    TutorialManager.Instance.NextPanel();
            }
        }

        if (HeadSpawner.Instance.isBonusLevel)
            return;

        if (lineBombCount > 0)
        {
            lineBombImage.raycastTarget = false;
            SoundManager.Instance.PlayBombCallSound();

            lineBombCount--;
            lineBombText.text = lineBombCount.ToString();

            itemController.UseLineBomb();
        }
        else
        {
            isPlaying = false;
            IsPlayingIngame = false;
            SoundManager.Instance.PlayUICloseSound();
            if (itemShopPanel)
                itemShopPanel.SetActive(true);
            else
                Utility.ErrorLog("item Shop Panel is not assigned in IngameUI.cs", 1);
        }
    }

    public void UseRadiusBomb()
    {
        if (TutorialManager.isTutorialRunning)
        {
            return;
        }

        if (HeadSpawner.Instance.isBonusLevel)
            return;

        if (radiusBombCount > 0)
        {
            radiusBombImage.raycastTarget = false;
            SoundManager.Instance.PlayBombCallSound();

            radiusBombCount--;
            radiusBombText.text = radiusBombCount.ToString();

            itemController.UseRadiusBomb();
        }
        else
        {
            isPlaying = false;
            IsPlayingIngame = false;
            if (itemShopPanel)
                itemShopPanel.SetActive(true);
            else
                Utility.ErrorLog("item Shop Panel is not assigned in IngameUI.cs", 1);
        }
    }

    public void UseTimeBomb()
    {
        if (TutorialManager.isTutorialRunning)
        {
            return;
        }

        if (timeBombCount > 0)
        {
            timeBombCount--;
            timeBombText.text = timeBombCount.ToString();

            itemController.UseTimeBomb();
        }
        else
        {
            isPlaying = false;
            IsPlayingIngame = false;
            if (itemShopPanel)
                itemShopPanel.SetActive(true);
            else
                Utility.ErrorLog("item Shop Panel is not assigned in IngameUI.cs", 1);
        }
    }

    public void GameStart()
    {
        isPlaying = true;
        IsPlayingIngame = true;

        GameManager.Instance.gameOver = false;
        SoundManager.Instance.PlayTappedSound();

        if (StartPanel)
            StartPanel.SetActive(false);
        else
            Utility.ErrorLog("Start Panel is not assigned in IngameUI.cs", 1);

        if (ingamePanel)
            ingamePanel.SetActive(true);
        else
            Utility.ErrorLog("Ingame Panel is not assigned in IngameUI.cs", 1);

        Time.timeScale = 1.0f;
        
        if (!TutorialManager.isTutorialRunning)
        {
            newHeadsPanel.SetActive(true);
            Time.timeScale = 0.0f;
        }

        ingameLevelText.text = GameManager.Instance.levelNumber.ToString();

        if (TutorialManager.isTutorialRunning)
        {
            tutorialPanel.SetActive(true);
            if (EncryptedPlayerPrefs.GetInt("Tutorial") == 0)
            {
                if (TutorialManager.Instance)
                    TutorialManager.Instance.OpenPanel(0);
                Time.timeScale = 0f;
            }
        }
        else
        {
            if (EncryptedPlayerPrefs.GetInt("RemoveAds") == 0)
            {
                ShowBanner(); //AdCall
            }
        }

        int limit = ((GameManager.Instance.levelNumber - 1) / levelsAfterWhichANewHeadIsSpawned) + 3;
        int limitPrevious = ((GameManager.Instance.levelNumber - 2) / levelsAfterWhichANewHeadIsSpawned) + 3;

        bool recentlyJustified = false;
        if (EncryptedPlayerPrefs.GetInt("RecentlyJustified", 0) == 1)
        {
            EncryptedPlayerPrefs.SetInt("RecentlyJustified", 0);
            recentlyJustified = true;
        }

        if ((limit == (limitPrevious + 1)) || recentlyJustified)
        {
            Time.timeScale = 0f;
            newHeadsPanel.SetActive(true);
            panelToOpenNext = newHeadsPanel;
        }

        if (GameManager.Instance.levelNumber == 1)
        {
            if (!EncryptedPlayerPrefs.HasKey("firstTimeNewHeadsPanel"))
            {
                EncryptedPlayerPrefs.SetInt("firstTimeNewHeadsPanel", 1);
                Time.timeScale = 0f;
                newHeadsPanel.SetActive(true);
                panelToOpenNext = newHeadsPanel;
            }
        }

        if (GameManager.Instance.levelNumber == levelsAfterWhichFallingHeadIsIntroduced)
        {
            if (!EncryptedPlayerPrefs.HasKey("firstTimeFallingHeadPanel"))
            {
                EncryptedPlayerPrefs.SetInt("firstTimeFallingHeadPanel", 1);
                Time.timeScale = 0f;
                fallingHeadIntroductionPanel.SetActive(true);
            }
        }

        if (GameManager.Instance.levelNumber == levelsAfterWhichDevilHeadIsIntroduced)
        {
            if (!EncryptedPlayerPrefs.HasKey("firstTimeDevilHeadPanel"))
            {
                EncryptedPlayerPrefs.SetInt("firstTimeDevilHeadPanel", 1);
                Time.timeScale = 0f;
                devilHeadIntroductionPanel.SetActive(true);
            }
        }

        if (GameManager.Instance.levelNumber == levelsAfterWhichIdGetterIsIntroduced)
        {
            if (!EncryptedPlayerPrefs.HasKey("firstTimeIdGetterdHeadPanel"))
            {
                EncryptedPlayerPrefs.SetInt("firstTimeIdGetterdHeadPanel", 1);
                Time.timeScale = 0f;
                idGetterHeadIntroductionPanel.SetActive(true);
            }
        }

        if (GameManager.Instance.levelNumber == levelsAfterWhichCopyHeadIsIntroduced)
        {
            if (!EncryptedPlayerPrefs.HasKey("firstTimeCopyHeadPanel"))
            {
                EncryptedPlayerPrefs.SetInt("firstTimeCopyHeadPanel", 1);
                Time.timeScale = 0f;
                copyHeadIntroductionPanel.SetActive(true);
            }
        }
        //EncryptedPlayerPrefs.DeleteKey("firstTimeFreezingHeadPanel");
        if (GameManager.Instance.levelNumber == levelsAfterWhichAFreezingHeadIsIntroduced)
        {
            if (!EncryptedPlayerPrefs.HasKey("firstTimeFreezingHeadPanel"))
            {
                EncryptedPlayerPrefs.SetInt("firstTimeFreezingHeadPanel", 1);

                Time.timeScale = 0f;
                freezingHeadIntroductionPanel.SetActive(true);
            }
        }

        if (HeadSpawner.Instance.isBonusLevel)
        {
            BounsLevelPanel.SetActive(true);    
        }

        Utility.MakeClickSound();
    }

    IEnumerator GiveExtraTimeCoins()
    {
        yield return new WaitForSeconds(2.2f);
        int timeLeft = (int)TimeController.Instance.TimeLeft;

        timeTakenSuccessText.GetComponent<SlideNumber>().SetNumberForTime(TimeController.Instance.TimeLeft);

        int currentFunds = totalFundsEarned;
        int newFunds = currentFunds + (timeLeft);

        //int totalFunds = EncryptedPlayerPrefs.GetInt("Funds");
        //totalFunds = totalFunds + newFunds;
        //EncryptedPlayerPrefs.SetInt("Funds", totalFunds);

        TimeLeftCoinsAnim.SetActive(true);
        StartCoroutine(StopCoinsAnim());

        //fundsEarnedSuccessText.GetComponent<SlideNumber>().AddToNumber(newFunds);
    }

    IEnumerator StopCoinsAnim()
    {
        yield return new WaitForSecondsRealtime(1.2f);
        
        TimeLeftCoinsAnim.SetActive(false);
    }

    public void CloseIntroPanels(GameObject panel)
    {
        if (!panelToOpenNext)
        {
            Time.timeScale = 1f;
        }
        panel.SetActive(false);
    }

    public void CloseNewHeadsPanel()
    {
        //ShowBanner();

        if (panelToOpenNext)
        {
            panelToOpenNext = null;
        }
        foreach (var item in pipeSpriteMasks)
        {
            item.enabled = true;
        }
        newHeadsPanel.SetActive(false);
        Time.timeScale = 1f;
        CheckForNewPanel();
    }

    public void CheckForNewPanel()
    {
        if (devilHeadIntroductionPanel.activeInHierarchy)
        {
            Time.timeScale = 0.0f;
        }  
        else
        if (fallingHeadIntroductionPanel.activeInHierarchy)
        {
            Time.timeScale = 0.0f;
        }
        else
        if (idGetterHeadIntroductionPanel.activeInHierarchy)
        {
            Time.timeScale = 0.0f;
        }
        else
        if (copyHeadIntroductionPanel.activeInHierarchy)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            return;
        }
    }

    public void GamePause()
    {

        isPlaying = false;
        IsPlayingIngame = false;
        GameIsOutOfIngameOperations(false);

        Time.timeScale = 0.0f;
        
        if (pausePanel)
            pausePanel.SetActive(true);
        else
            Utility.ErrorLog("Pause Panel is not assigned in IngameUI.cs", 1);

        SoundManager.Instance.PlayPauseSound();
        ShowInterstitialAd();
        Debug.LogError("Interstitial");

    }

    public void GameResume()
    {
        isPlaying = true;
        IsPlayingIngame = true;

        if (pausePanel)
            pausePanel.SetActive(false);
        else
            Utility.ErrorLog("Pause Panel is not assigned in IngameUI.cs", 1);

        Time.timeScale = 1.0f;

        Utility.MakeClickSound();
        ShowInterstitialAd();
        Debug.LogError("Interstitial");

    }

    public void BackToMainMenu()
    {
        HideBanner();

        ShowInterstitialAd();

        UpdateDataOfGame();
        GameManager.Instance.comingFromIngame = true;

        Screen.orientation = ScreenOrientation.Portrait;

        StartLoading();
    }

    public void GameRestart()
    {
        ShowInterstitialAd();
        UpdateDataOfGame();
        if (loadingPanel)
        {
            loadingPanel.SetActive(true);

            if (loadingPanel.GetComponent<LoadScene>())
            {
                loadingPanel.GetComponent<LoadScene>().LoadTheSceneAgain();
            }
            else
                Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
        }
        else
            Utility.ErrorLog("Loading Panel Panel is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void GameNext()
    {
        ShowInterstitialAd();
        UpdateDataOfGame();
        GameManager.Instance.levelNumber += 1;

        if (loadingPanel)
        {
            loadingPanel.SetActive(true);

            if (loadingPanel.GetComponent<LoadScene>())
            {
                loadingPanel.GetComponent<LoadScene>().LoadTheSceneAgain();
            }
            else
                Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
        }
        else
            Utility.ErrorLog("Loading Panel Panel is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void GameComplete(bool gameStatus)
    {
        HideBanner();

        isPlaying = false;
        GameManager.Instance.gameOver = true;
        HeadSpawner.Instance.SetForJustification();
        GameIsOutOfIngameOperations(false);
        
        if (gameStatus)
        {
            TimeController.Instance.StopTimer();
            if (HeadSpawner.Instance.isBossLevel)
            {
                string pref = "BossLevelNumber" + HeadSpawner.Instance.bossIndexNumber;
                EncryptedPlayerPrefs.SetInt(pref, 1);
            }
            ScoreCalculation(gameStatus);
            StartCoroutine(ShowPanelWait(gameStatus));
        }
        else
        {
            if (AdManager.Instance)
            {
                if ((AdManager.Instance.isRewardVideoAvailable && RewardVideo.pressed != 2))
                {
                    ReSpawnPanel.SetActive(true);
                }
            }
            else
            {
                ScoreCalculation(gameStatus);
                StartCoroutine(ShowPanelWait(gameStatus));
            }
        }
    }

    public void RevivePlayer()
    {
        ReSpawnPanel.GetComponent<RespawnPanel>().RespawnByVideo();
        Time.timeScale = 1;
        isPlaying = true;
        GameManager.Instance.gameOver = false;
        TimeController.Instance.AddTimeInMinutes(2);
        TimeController.Instance.ResetTimeCompleted();
        HeadPositionsArray.Instance.BlastNumberOfHeads(15);
        HeadSpawner.Instance.StartSpawningAfterTime();
    }
    public void FailOperations()
    {
        //ShowInterstitialAd();
        ScoreCalculation(false);
        hasDataBeenUpdated = true;
        IsPlayingIngame = false;
        failPanel.SetActive(true);
    }
    public void UpdateHeaderValues()
    {
        DisplayItemValues[] items = GameObject.FindObjectsOfType<DisplayItemValues>() as DisplayItemValues[];

        foreach (var item in items)
        {
            item.ShowCount();
        }
    }

    IEnumerator ShowPanelWait(bool gameStatus)
    {
        AdjustLowVolume();

        yield return new WaitForSeconds(2f);

        ShowInterstitialAd();
        yield return new WaitForSeconds(1f);

        //Time.timeScale = 0f;

        hasDataBeenUpdated = true;
        IsPlayingIngame = false;

        if (gameStatus)
        {
            //if (justifiedHeadsRewardsPanel && HeadSpawner.Instance.isBonusLevel)
            //{
            //    justifiedHeadsRewardsPanel.SetActive(true);
            //    justifiedHeadsRewardsPanel.GetComponentInChildren<JustifiedHeadReward>().ShowJustifiedRewards(2);
            //}
            if (successPanel)
            {
                successPanel.SetActive(true);
                GameManager.Instance.environmentType = Random.Range(0, GameInitializer.Instance.headPositionsInstances.Length);
                //yield return new WaitForSeconds(2f);
                StartCoroutine(GiveExtraTimeCoins());
            }
            else
                Utility.ErrorLog("Success Panel is not assigned in IngameUI.cs", 1);
        }
        else
        {
            if (failPanel)
            {
                failPanel.SetActive(true);
                //StartCoroutine(GiveExtraTimeCoins());
            }
            else
                Utility.ErrorLog("Fail Panel is not assigned in IngameUI.cs", 1);
        }
    }

    public void ClaimJustifiedReward()
    {
        justifiedHeadsRewardsPanel.SetActive(false);
        justifyHeadPanel.SetActive(false);
       
        if (successPanel.activeInHierarchy)
        {
            successPanel.GetComponent<Animator>().SetTrigger("showBtns");
        }
        else
        if (failPanel.activeInHierarchy)
        {
            failPanel.GetComponent<Animator>().SetTrigger("showBtns");
        }
        UpdateHeaderValues();
        Utility.MakeClickSound();
        //FailCryptoPanel.SetActive(true);
        //SuccessCryptoPanel.SetActive(true);
    }

    public void PlayGiveCoinsAnimation()
    {
        GameObject coinsAnim = Instantiate(giveCoinsAnimation);
        coinsAnim.transform.parent = ingamePanel.gameObject.transform;
        coinsAnim.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    public void CloseCryptoInEndGame()
    {
        FailCryptoPanel.SetActive(false);
        SuccessCryptoPanel.SetActive(false);
    }
    public void StartLoading()
    {
        if (loadingPanel)
        {
            loadingPanel.SetActive(true);

            if (loadingPanel.GetComponent<LoadScene>())
            {
                loadingPanel.GetComponent<LoadScene>().StartLoadingScene("MainMenu");
            }
            else
                Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
        }
        else
            Utility.ErrorLog("Loading Panel is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void GameIsOutOfIngameOperations(bool dateCheckDependencyForInterstitial)
    {
        //ShowInterstitialAd(dateCheckDependencyForInterstitial);
        //ShowBanner();
    }
    private static int adCounter = 0;
    public void ShowInterstitialAd()
    {
        //if (adCounter != 2)
        //{
        //    adCounter++;
        //    return;
        //}
        //else
        //{
        //    adCounter = 0;
        //}

        if (AdManager.Instance)
        {
            if (EncryptedPlayerPrefs.GetInt("RemoveAds") == 0)
            {
                AdManager.Instance.ShowInterstitialAd();
            }
        }
    }

    public void ShowBanner()
    {

        if (AdManager.Instance)
        {
            if (EncryptedPlayerPrefs.GetInt("RemoveAds") == 0)
            {
                AdManager.Instance.ShowBannerAd();
            }
        }
    }

    public void HideBanner()
    {
        if (AdManager.Instance) 
        {
            AdManager.Instance.HideBannerAd();
        }
    }

    public void MoreThanThreeHeads()
    {
        GameObject fourX = Instantiate(fourXAnimation);
        fourX.transform.parent = this.transform;
        fourX.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    public void ShareGame()
    {
        if (AdManager.Instance)
        {
            if (AdManager.Instance.gameObject.GetComponent<Share>())
            {
                AdManager.Instance.gameObject.GetComponent<Share>().ShareGame();
            }
            else
                Utility.ErrorLog("Share Component not found in on " + AdManager.Instance.gameObject, 2);
        }

        Utility.MakeClickSound();
    }

    public void RateUs()
    {
        string url = "https://play.google.com/store/apps/details?id=" + Application.identifier;
        Application.OpenURL(url);
    }

    public void DoubleTheCoins()
    {
        int totalFunds = EncryptedPlayerPrefs.GetInt("Funds", 0);
        totalFunds = totalFunds + totalFundsEarned;
        EncryptedPlayerPrefs.SetInt("Funds", totalFunds);
        //int totalFunds =  EncryptedPlayerPrefs.GetInt("Funds");
        //totalFundsEarned = totalFundsEarned * 2;
        //totalFunds = totalFunds + totalFundsEarned;
        //EncryptedPlayerPrefs.SetInt("Funds", totalFunds);
        if (fundsEarnedSuccessText)
        {
            fundsEarnedSuccessText.text = (totalFundsEarned * 2).ToString();
        }
        if (rewardText)
        {
            rewardText.text = (totalFundsEarned * 2).ToString();
        }
        else
            Utility.ErrorLog("rewardText is not assigned in IngameUI.cs", 1);

        if (RewardPanel)
        {
            RewardPanel.SetActive(true);
        }
        else
            Utility.ErrorLog("RewardPanel is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void RewardPanelClose()
    {
        if (RewardPanel)
        {
            RewardPanel.SetActive(false);
        }
        else
            Utility.ErrorLog("RewardPanel is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void RewardNotAvailablePanelClose()
    {
        if (rewardNotAvailable)
        {
            rewardNotAvailable.SetActive(false);
        }
        else
            Utility.ErrorLog("rewardNotAvailable is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void RewardLostPanelClose()
    {
        if (rewardLost)
        {
            rewardLost.SetActive(false);
        }
        else
            Utility.ErrorLog("rewardLost is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }

    private void ScoreCalculation(bool gameWon)
    {
        UpdateDataOfGame();

        if (gameWon)
        {
            HeartBeatSound.Instance.StopHeartBeatSound();
            LevelCompleteMusic.Instance.PlaySuccessMusic();

            int totalUnlockedLevels = EncryptedPlayerPrefs.GetInt("LevelsUnocked");
            if (GameManager.Instance.levelNumber >= totalUnlockedLevels)
            {
                totalUnlockedLevels++;
            }
            EncryptedPlayerPrefs.SetInt("LevelsUnocked", totalUnlockedLevels);

            int funds = EncryptedPlayerPrefs.GetInt("Funds");
            int tempFunds = Random.Range(10, 25);
            funds = tempFunds + funds;
            EncryptedPlayerPrefs.SetInt("Funds", funds);

            fundsEarnedIngameText.text = funds.ToString();

            totalFundsEarned += tempFunds;

            if (EncryptedPlayerPrefs.GetString("HighScore") == "")
            {
                EncryptedPlayerPrefs.SetString("HighScore", "0");
            }

            ulong gameHighScore = ulong.Parse(EncryptedPlayerPrefs.GetString("HighScore"));
            ulong gameHighScoreWeekly = ulong.Parse(EncryptedPlayerPrefs.GetString("HighScoreWeekly"));

            int scoreLevel = (totalFundsEarned * 2) + (100 * GameManager.Instance.eyesPopped) + 500;
            gameScore += (ulong)scoreLevel;
            gameHighScore += gameScore;
            gameHighScoreWeekly += gameScore;

            //if (EncryptedPlayerPrefs.GetInt("Highscore", 0) < gameScore)
            //{
            EncryptedPlayerPrefs.SetString("Highscore", gameHighScore.ToString());
            EncryptedPlayerPrefs.SetString("HighScoreWeekly", gameHighScoreWeekly.ToString());
            //}

            if (GameServerData.Instance)
                GameServerData.Instance.SetData(gameHighScore.ToString(), gameHighScoreWeekly.ToString(), 1);

            if (fundsEarnedSuccessText)
            {
                fundsEarnedSuccessText.text = totalFundsEarned.ToString();
            }
            else
                Utility.ErrorLog("fundsEarnedSuccessText is not assigned in IngameUI.cs", 1);

            headsPoppedSuccessText.text = numberOfHeadsPopped.ToString();
            timeTakenSuccessText.text = TimeController.Instance.GetTime();
        }
        else
        {
            HeartBeatSound.Instance.StopHeartBeatSound();
            LevelCompleteMusic.Instance.PlayFailMusic();

            if (fundsEarnedFailText)
            {
                fundsEarnedFailText.text = totalFundsEarned.ToString();
            }
            else
                Utility.ErrorLog("fundsEarnedFailText is not assigned in IngameUI.cs", 1);

            headsPoppedFailText.text = numberOfHeadsPopped.ToString();
            timeTakenFailText.text = TimeController.Instance.GetTime();
        }
    }

    public void RemoveAds()
    {
        if (AdManager.Instance)
        {
            GameObject adManager = AdManager.Instance.gameObject;
            if (adManager.GetComponent("Purchaser"))
            {
                adManager.GetComponent("Purchaser").SendMessage("RemoveAdsPurchaseCompleted");
            }
            else
                Utility.ErrorLog("Purchaser is not found in IngameUI.cs " + " of " + this.gameObject.name, 2);
        }
        Utility.MakeClickSound();
    }

    public void MakeClickSound()
    {
        if (_soundSource)
        {
            _soundSource.PlayUIClickedSound();
        }
        else
            Utility.ErrorLog("Sound Source is not assigned in IngameUI.cs", 1);
    }

    public static void PlayMusic()
    {
        if (_musicSource)
        {
            _musicSource.PlayIngameMusic();
        }
        else
            Utility.ErrorLog("Music Source is not assigned in IngameUI.cs", 1);
    }

    public void ControlViberation()
    {
        if (GameManager.Instance.vibrationValue == 1)
        {
            GameManager.Instance.vibrationValue = 0;
            EncryptedPlayerPrefs.SetInt("vibrationValue", 0);
        }
        else 
        if (GameManager.Instance.vibrationValue == 0)
        {
            //Vibration.Vibrate(50);
            GameManager.Instance.vibrationValue = 1;
            EncryptedPlayerPrefs.SetInt("vibrationValue", 1);
        }
        ChangeVibrationSprite();
        Utility.MakeClickSound();
    }

    public void ChangeVibrationSprite()
    {
        if (GameManager.Instance.vibrationValue == 1)
        {
            vibrationObj.sprite = vibrationOn;
        }
        else
        if (GameManager.Instance.vibrationValue == 0)
        {
            vibrationObj.sprite = vibrationOff;
        }
    }

    public void AdjustLowVolume()
    {
        musicSource.GetComponent<SoundAndMusic>().MusicLowAdjust(musicLowValue);
    }

    public void AdjustHighVolume()
    {
        musicSource.GetComponent<SoundAndMusic>().MusicHighAdjust();
    }
}