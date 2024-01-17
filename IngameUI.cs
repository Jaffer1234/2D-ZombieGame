using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    [Header("URL")]
    public string iosAppURL;
    public string privacyURL;

    [Header("SOUN AND MUSIC")]
    public SoundManager soundSource;
    public MusicManager musicSource;

    public static SoundManager _soundSource;
    public static MusicManager _musicSource;

    public static bool _isPlaying = false;
    
    [Header("PUBLIC MENU PANELS")]
    public GameObject StartPanel;
    public GameObject settingsPanel;
    public GameObject itemsPanel;
    public GameObject ingamePanel;
    public GameObject pausePanel;
    public GameObject failPanel;
    public GameObject successPanel;
    public GameObject itemStorePanel;
    public GameObject InsufficientFundsPanel;
    public GameObject loadingPanel;
    public GameObject RewardPanel;
    public GameObject rewardNotAvailable;
    public GameObject rewardLost;
    public GameObject tutorialPanel;
    public GameObject logoSplashScreenPanel;
    public GameObject playerInputPanel;
    public GameObject GDRPConsentPanel;
    public GameObject leadboardPanel;
    public GameObject serverConnectionErrorPanel;
    public GameObject languagesPanel;
    public GameObject splashScreenPanel;
    public GameObject pleaseWaitPanel;
    public GameObject fortuneWheelPanel;
    public GameObject dailyRewardPanel;
    public GameObject prizeShowPanel;
    public GameObject revivePanel;
    public GameObject fillInformationForPrizePanel;

    [Header("PUBLIC TEXT FIELDS")]
    public Text lineBombText;
    public Text radiusBombText;
    public Text ingameLevelText;
    public Text rewardText;
    public Text fundsEarnedIngameText;
    public Text distanceCoveredFailText;
    public Text timeTakenFailText;
    public Text fundsEarnedFailText;
    public Text scoreFailText;
    public Text gameScoreText;

    public Text serverConnectionErrorPanelText;

    [Header("PUBLIC IMAGE FIRLDS")]
    public Image healthFillBar;


    public Animator advertiserPanel;

    //[Header("VIBRATION SPRITES")]
    //[SerializeField] private Sprite vibrationOn;
    //[SerializeField] private Sprite vibrationOff;
    //[SerializeField] private Image vibrationObj;

    public float musicLowValue = 0.25f;

    private int totalScoreAddedFromEnv = 0;
    private int totalFundsEarned;
    private int totalFundsAtStart = 0;
    private ItemController itemController;
    private GameObject panelToOpenNext = null;

    public static IngameUI Instance;

    //[Header("PARTICLES")]
    //public GameObject[] menuParticles;
    public GameObject[] languageTicks;


    public GameObject fortuneWheelMarkBlinker;
    public GameObject dailyRewardCanvasPanel;

    private bool isOutOfFocus = false;
    private bool hasDataBeenUpdated = false;
    public static bool IsPlayingIngame = false;

    private bool adPanelOpen = false;


    private ulong gameScore = 0;

    public delegate void ApplicationOutOfFocusOperations();
    public static ApplicationOutOfFocusOperations applicationUnfocus;
    public delegate void ApplicationInFocusOperations();
    public static ApplicationInFocusOperations applicationfocus;

    void Awake()
    {
        //EncryptedPlayerPrefs.SetInt("Funds", 9999999);
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _soundSource = soundSource;
        _musicSource = musicSource;


        SetEncryptedPlayerPrefs();
    }

    void Start ()
    {
        if (!EncryptedPlayerPrefs.HasKey("FixAdsBug"))
        {
            EncryptedPlayerPrefs.SetInt("FixAdsBug", 1);
        }

        if (!EncryptedPlayerPrefs.HasKey("RemoveAds"))
        {
            EncryptedPlayerPrefs.SetInt("RemoveAds", 0);
        }

        Time.timeScale = 1f;

        IsPlayingIngame = false;

        _isPlaying = false;

        //ChangeVibrationSprite();

        EnvironmentManager.Instance.ChangeSky();

        if (GDRPConsentPanel)
            GDRPConsentPanel.SetActive(false);

        if(GameManager.Instance.comingFromIngameRestart)
        {
            GameManager.Instance.comingFromIngameRestart = false;
            if (StartPanel)
            {
                StartPanel.SetActive(false);
            }
            else
                Utility.ErrorLog("Start Panel is not assigned in IngameUI.cs", 1);

            MusicManager.Instance.LerpSoundVolume(false);
            GameStart();

            ShowBannerIfNotBeingShownAlready();
            //PlayMusic();
        }
        else if (GameManager.Instance.comingFromIngame)
        {
            GameManager.Instance.comingFromIngame = false;
            if (StartPanel)
            {
                StartPanel.SetActive(true);
            }
            else
                Utility.ErrorLog("Start Panel is not assigned in IngameUI.cs", 1);

            MusicManager.Instance.LerpSoundVolume(false);
            GameIsOutOfIngameOperations(/*true*/);

            //ShowBannerIfNotBeingShownAlready();
            PlayMusic();
        }
        else
        {
            if (StartPanel)
            {
                StartPanel.SetActive(false);
            }
            else
                Utility.ErrorLog("Start Panel is not assigned in IngameUI.cs", 1);

            if (logoSplashScreenPanel)
            {
                logoSplashScreenPanel.SetActive(true);
                StartCoroutine(logoScren());
            }
            GameIsOutOfIngameOperations(/*true*/);
        }

        //foreach (var item in languageTicks)
        //{
        //    item.SetActive(false);
        //}
        //if (EncryptedPlayerPrefs.GetInt("LanguageSelected") != -1)
        //{
        //    languageTicks[EncryptedPlayerPrefs.GetInt("LanguageSelected")].SetActive(true);
        //}

        if (ItemController.Instance)
        {
            itemController = ItemController.Instance;
        }
        else
            Utility.ErrorLog("ItemController could not be found in IngameUI.cs of " + this.gameObject, 1);

        
        fundsEarnedIngameText.text = EncryptedPlayerPrefs.GetInt("Funds").ToString();


        revivePanel.SetActive(false);

        totalFundsEarned = 0;

        totalScoreAddedFromEnv = 1;
        UpdateScore(0);
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
        }
    }

    IEnumerator logoScren()
    {
        yield return new WaitForSeconds(2.5f);
        logoSplashScreenPanel.SetActive(false);

        PlayMusic();

        if (EncryptedPlayerPrefs.GetInt("playerNameSet") == 0 && EncryptedPlayerPrefs.GetInt("playerGuestNameSet") == 0)
        {
            splashScreenPanel.SetActive(true);
        }
        else
        {
            splashScreenPanel.SetActive(false);
            CheckForLanguages();
        }
    }
    public void CancelInputPanel()
    {
        //splashScreenPanel.SetActive(true);
        playerInputPanel.SetActive(false);
        Utility.MakeClickSound();
    }

    public void SetNumberOfGuestInName(int number)
    {
        EncryptedPlayerPrefs.SetString("PlayerGuestName", ("Guest" + number));
        EncryptedPlayerPrefs.SetInt("playerGuestNameSet", 1);
        GameServerData.Instance.SetData("0", "0", 1);
        CheckForLanguages();
    }
    public void SetUniqueCodeInName()
    {
        string tempName = "Guest" + MakeUniqueCode();
        EncryptedPlayerPrefs.SetString("PlayerGuestName", tempName);
        EncryptedPlayerPrefs.SetInt("playerGuestNameSet", 1);
        GameServerData.Instance.SetData("0", "0", 1);
        CheckForLanguages();
    }
    public void SetGuestUsername()
    {
        if (EncryptedPlayerPrefs.GetInt("playerGuestNameSet") == 0)
        {
            EncryptedPlayerPrefs.SetInt("playerGuestNameSet", 1);
            GameServerData.Instance.GetNumberOfRecords();
        }
    }
    public void SignUp()
    {
        TakePlayerUsernameInput();
    }
    public string MakeUniqueCode()
    {
        string characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ!()@^";
        string code = "";
        for (int i = 0; i < 20; i++)
        {
            int a = Random.Range(0, characters.Length);
            code += characters[a];
        }
        return code;
    }
    public void CheckForLanguages()
    {
        if (EncryptedPlayerPrefs.GetInt("LanguageSelected") == -1)
        {
            foreach (var item in languageTicks)
            {
                item.SetActive(false);
            }
            OpenLanguagesPanel();
        }
        else
        {
            SetGDRP();
        }
    }
    public void CheckForLanguageInputFirstTime(int languageNumber)
    {
        EncryptedPlayerPrefs.SetInt("LanguageSelected", languageNumber);
        foreach (var item in languageTicks)
        {
            item.SetActive(false);
        }
        languageTicks[languageNumber].SetActive(true);

        if (EncryptedPlayerPrefs.GetInt("LanguageSelected") == -1)
        {
            languagesPanel.SetActive(true);
        }
        else
        {
            SetGDRP();
        }
    }

    public void openWeeklyRewardPanel()
    {
        dailyRewardCanvasPanel.SetActive(true);
        TurnParticlesOff();
        Utility.MakeClickSound();
    }

    public void OpenLanguagesPanel()
    {
        languagesPanel.SetActive(true);
        MainMenuCloseOperations();
        StartPanel.SetActive(false);
        Utility.MakeClickSound();
    }


    public void UpdateHealthImage(int maxHealth, int currentHealth)
    {
        float fillAmount = (float)currentHealth / (float)maxHealth;

        if (fillAmount > 1)
        {
            fillAmount = 1f;
        }
        if (fillAmount < 0)
        {
            fillAmount = 0f;
        }
        healthFillBar.fillAmount = fillAmount;
    }
    public void OpenSettingPanel()
    {
        if (settingsPanel)
        {
            settingsPanel.SetActive(true);
            StartPanel.SetActive(false);
            MainMenuCloseOperations();
            //settingsPanel.GetComponentInChildren<Animator>().SetBool("open", true);
        }
        //else
        //    Utility.ErrorLog("Settings Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void BackFromSettings()
    {
        StartPanel.SetActive(true);
        settingsPanel.SetActive(false);
        MainMenuOpenOperations();
        //if (settingsPanel)
        //{
        //    settingsPanel.GetComponentInChildren<Animator>().SetBool("open", false);

        //    StartCoroutine(closePanel());
        //}
        //else
        //    Utility.ErrorLog("Settings Panel is not assigned in MainMenuUI.cs", 1);

        SoundManager.Instance.PlayUICloseSound();
    }

    //IEnumerator closePanel()
    //{
    //    yield return new WaitForSeconds(1.5f);

    //    settingsPanel.SetActive(false);
    //}
    public void AddScore(int score)
    {
        totalScoreAddedFromEnv += score;
    }

    public void UpdateScore(int timeScore)
    {
        if (gameScoreText)
        {
            //gameScoreText.gameObject.GetComponent<Animator>().SetTrigger("add");

            int totalGameScore = GetScore(timeScore);
            gameScore = (ulong)totalGameScore;
            gameScoreText.text = totalGameScore.ToString();
        }
    }
    public void UpdateItemValuesCount()
    {
    }

    //public void UseLineBomb()
    //{
    //    if (TutorialManager.isTutorialRunning)
    //    {
    //        if (EncryptedPlayerPrefs.GetInt("Tutorial") == 3)
    //        {
    //            if (TutorialManager.Instance)
    //                TutorialManager.Instance.NextPanel();
    //        }
    //    }

    //    if (lineBombCount > 0)
    //    {
    //        itemController.UseLineBomb();
    //    }
    //    else
    //    {
    //        _isPlaying = false;
    //        SoundManager.Instance.PlayUICloseSound();
    //        if (itemStorePanel)
    //            itemStorePanel.SetActive(true);
    //        else
    //            Utility.ErrorLog("item Shop Panel is not assigned in IngameUI.cs", 1);
    //    }
    //}

    //public void UseRadiusBomb()
    //{
    //    if (TutorialManager.isTutorialRunning)
    //    {
    //        return;
    //    }

    //    if (radiusBombCount > 0)
    //    {
    //        itemController.UseRadiusBomb();
    //    }
    //    else
    //    {
    //        _isPlaying = false;
    //        if (itemStorePanel)
    //            itemStorePanel.SetActive(true);
    //        else
    //            Utility.ErrorLog("item Shop Panel is not assigned in IngameUI.cs", 1);
    //    }
    //}

    //public void UseTimeBomb()
    //{
    //    if (TutorialManager.isTutorialRunning)
    //    {
    //        return;
    //    }

    //    if (timeBombCount > 0)
    //    {
    //        itemController.UseTimeBomb();
    //    }
    //    else
    //    {
    //        _isPlaying = false;
    //        if (itemStorePanel)
    //            itemStorePanel.SetActive(true);
    //        else
    //            Utility.ErrorLog("item Shop Panel is not assigned in IngameUI.cs", 1);
    //    }
    //}

    public void GameStart()
    {
        ShowBanner();

        _isPlaying = true;

        IsPlayingIngame = true;

        totalFundsAtStart = EncryptedPlayerPrefs.GetInt("Funds");

        GameManager.Instance.gameOver = false;

        PlayMusicIngame();

        SoundManager.Instance.PlayTappedSound();
        GameIsInOFInGameOperations();

        if (StartPanel)
            StartPanel.SetActive(false);
        else
            Utility.ErrorLog("Start Panel is not assigned in IngameUI.cs", 1);

        if (ingamePanel)
            ingamePanel.SetActive(true);
        else
            Utility.ErrorLog("Ingame Panel is not assigned in IngameUI.cs", 1);

        Time.timeScale = 1.0f;
        
        ingameLevelText.text = GameManager.Instance.levelNumber.ToString();


        GameInitializer.Instance.SpawnSelectedPlayer();

        //if (TutorialManager.isTutorialRunning)
        //{
        //    tutorialPanel.SetActive(true);
        //    if (EncryptedPlayerPrefs.GetInt("Tutorial") == 0)
        //    {
        //        if (TutorialManager.Instance) 
        //            TutorialManager.Instance.OpenPanel(0);
        //        Time.timeScale = 0f;
        //    }
        //}

        //ShowInterstitialAd(false);
        Utility.MakeClickSound();
    }

    public void CloseIntroPanels(GameObject panel)
    {
        if (!panelToOpenNext)
        {
            Time.timeScale = 1f;
        }
        panel.SetActive(false);
    }

    public void GamePause()
    {
        ShowInterstitialAd(false);
        IsPlayingIngame = false;
        _isPlaying = false;
        GameIsOutOfIngameOperations(/*false*/);
        
        Time.timeScale = 0.0f;
        HideBanner();

        if (pausePanel)
            pausePanel.SetActive(true);
        else
            Utility.ErrorLog("Pause Panel is not assigned in IngameUI.cs", 1);

        SoundManager.Instance.PlayPauseSound();
    }

    public void GameResume()
    {
        ShowBanner();

        IsPlayingIngame = true;
        _isPlaying = true;
        GameIsInOFInGameOperations();

        if (pausePanel)
            pausePanel.SetActive(false);
        else
            Utility.ErrorLog("Pause Panel is not assigned in IngameUI.cs", 1);

        Time.timeScale = 1.0f;

        Utility.MakeClickSound();
    }

    public void BackToMainMenu()
    {
        HideBanner();

        UpdateDataOfGame();
        GameManager.Instance.comingFromIngame = true;
        Screen.orientation = ScreenOrientation.Portrait;

        //_isPlaying = true;

        GameManager.Instance.gameOver = true;

        SoundManager.Instance.PlayTappedSound();

        GameIsOutOfIngameOperations();

        StartLoading();
    }

    public void GameRestart()
    {
        HideBanner();

        UpdateDataOfGame();
        GameManager.Instance.comingFromIngameRestart = true;

        GameManager.Instance.gameOver = true;

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
    public void FailOperations()
    {
        MusicManager.Instance.LerpSoundVolume(false);
        PlayMusic();

        ShowInterstitialAd(false);
        ScoreCalculation(false);
        hasDataBeenUpdated = true;
        IsPlayingIngame = false;
        revivePanel.SetActive(false);
        failPanel.SetActive(true);
    }
    public void GameComplete(bool gameStatus)
    {
        _isPlaying = false;
        GameManager.Instance.gameOver = true;

        GameIsOutOfIngameOperations(/*false*/);

        if (gameStatus)
        {
            ScoreCalculation(gameStatus);
            StartCoroutine(ShowPanelWait(gameStatus));
        }
        else
        {
            if (AdManager.Instance)
            {
                if ((AdManager.Instance.isRewardVideoAvailable/* && RewardVideo.pressed != 2*/))
                {
                    revivePanel.SetActive(true);
                }
            }
            else
            {
                ScoreCalculation(gameStatus);
                StartCoroutine(ShowPanelWait(gameStatus));
            }
        }
    }
    public void ShowLeaderboard()
    {
        MainMenuCloseOperations();
        GameServerData.Instance.GetData(200);
        Utility.MakeClickSound();
    }
    public void CancelPleaseWait()
    {
        GameServerData.Instance.SendCallStopAllCoroutines();
        pleaseWaitPanel.SetActive(false);
        serverConnectionErrorPanel.SetActive(false);
    }
    public void CloseErrorPanel()
    {
        //GameObject prevPanel = HighscoreManager.Instance.GetPreviousPanel();
        //if (prevPanel != null)
        //{
        //    prevPanel.SetActive(true);
        //}
        MainMenuOpenOperations();
        serverConnectionErrorPanel.SetActive(false);
        Utility.MakeClickSound();
    }
    public void UpdateHeaderValues()
    {
        DisplayItemValues[] items = GameObject.FindObjectsOfType<DisplayItemValues>() as DisplayItemValues[];
        foreach (var item in items)
        {
            item.ShowCount();
        }
    }
    public void StopEverything()
    {
        if (MoveTowards.movingInstances != null)
        {
            if (MoveTowards.movingInstances.Count > 0)
            {
                foreach (var item in MoveTowards.movingInstances)
                {
                    if (item.gameObject.GetComponentInChildren<Animator>())
                    {
                        item.gameObject.GetComponentInChildren<Animator>().enabled = false; 
                    }
                }
            }
        }
        PlayerController.speedMultiplier = 0;
    }

    IEnumerator ShowPanelWait(bool gameStatus)
    {
        AdjustLowVolume();

        yield return new WaitForSeconds(2f);

        IsPlayingIngame = false;

        hasDataBeenUpdated = true;

        if (gameStatus)
        {
            if (successPanel)
            {
                successPanel.SetActive(true);
            }
            else
                Utility.ErrorLog("Success Panel is not assigned in IngameUI.cs", 1);
        }
        else
        {
            StopEverything();
            if (failPanel)
            {
                MusicManager.Instance.LerpSoundVolume(false);
                PlayMusic();
                failPanel.SetActive(true);
            }
            else
                Utility.ErrorLog("Fail Panel is not assigned in IngameUI.cs", 1);
        }
    }
    public void StartLoading()
    {
        if (loadingPanel)
        {
            loadingPanel.SetActive(true);

            if (loadingPanel.GetComponent<LoadScene>())
            {
                loadingPanel.GetComponent<LoadScene>().StartLoadingScene("Ingame");
            }
            else
                Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
        }
        else
            Utility.ErrorLog("Loading Panel is not assigned in IngameUI.cs", 1);

        Utility.MakeClickSound();
    }
    public void GameIsInOFInGameOperations()
    {
        dailyRewardPanel.SetActive(false);
        //HideBanner();
    }

    public void SwitchAdPanel()
    {
        if (adPanelOpen)
        {
            adPanelOpen = false;
            advertiserPanel.SetBool("open", false);
        }
        else
        {
            adPanelOpen = true;
            advertiserPanel.SetBool("open", true);
        }
        Utility.MakeClickSound();
    }
    public void GameIsOutOfIngameOperations(/*bool dateCheckDependencyForInterstitial*/)
    {
        dailyRewardPanel.SetActive(true);
        //ShowInterstitialAd(dateCheckDependencyForInterstitial);
        //ShowBanner();
    }

    public void ShowInterstitialAd(bool dateCheckDependency)
    {
        if (AdManager.Instance)
        {
            if (AdManager.Instance)
            {
                if (EncryptedPlayerPrefs.GetInt("RemoveAds") == 0)
                {
                    AdManager.Instance.ShowInterstitialAd();
                }
            }
            //AdManager.Instance.ShowInterstitialAd(dateCheckDependency);//aliedit
        }
    }

    public void ShowBanner()
    {
        if (AdManager.Instance)
        {
            if (AdManager.Instance)
            {
                if (EncryptedPlayerPrefs.GetInt("RemoveAds") == 0)
                {
                    AdManager.Instance.ShowBannerAd();
                }
            }
        }
    }

    public void ShowBannerIfNotBeingShownAlready()
    {
        if (AdManager.Instance)
        {
            //AdManager.Instance.ShowBannerIfNotBeingShownAlready();//aliedit
        }
    }

    public void HideBanner()
    {
        if (AdManager.Instance)
        {
            if (AdManager.Instance)
            {
                AdManager.Instance.HideBannerAd();
            }
        }
    }

    public void RevivePlayer()
    {
        revivePanel.GetComponent<RespawnPanel>().RespawnByVideo();
        Time.timeScale = 1;
        _isPlaying = true;
        IsPlayingIngame = true;
        GameManager.Instance.gameOver = false;
        MusicManager.Instance.LerpSoundVolume(false);
        PlayerStaticInstance.Instance.gameObject.GetComponent<PlayerController>().PlayerReset(true);
        PlayerStaticInstance.Instance.gameObject.GetComponent<BodyPartsReferences>().AttachBody();
        PlayerController.speedMultiplier = 1;
        GameInitializer.Instance.SpawnObject();
    }

    public void ShareGame()
    {
        //if (AdManager.Instance)
        //{
        //    if (AdManager.Instance.gameObject.GetComponent<Share>())
        //    {
        //        AdManager.Instance.gameObject.GetComponent<Share>().ShareGame();
        //    }
        //    else
        //        Utility.ErrorLog("Share Component not found in on " + AdManager.Instance.gameObject, 2);
        //}

        //Utility.MakeClickSound();
    }

    public void RateUs()
    {
        string url = "https://play.google.com/store/apps/details?id=" + Application.identifier;
        Application.OpenURL(url);
    }

    public void DoubleTheCoins()
    {
        int totalFunds = EncryptedPlayerPrefs.GetInt("Funds", 0);
        totalFunds = totalFunds + (totalFundsEarned * 2);
        EncryptedPlayerPrefs.SetInt("Funds", totalFunds);

        //if (fundsEarnedSuccessText)
        //{
        //    fundsEarnedSuccessText.text = (totalFundsEarned * 2).ToString();
        //}
        if (fundsEarnedFailText)
        {
            fundsEarnedFailText.text = (totalFundsEarned * 2).ToString();
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
            LevelCompleteMusic.Instance.PlaySuccessMusic();

            int totalUnlockedLevels = EncryptedPlayerPrefs.GetInt("LevelsUnocked");
            if (GameManager.Instance.levelNumber >= totalUnlockedLevels)
            {
                totalUnlockedLevels++;
            }
            EncryptedPlayerPrefs.SetInt("LevelsUnocked", totalUnlockedLevels);

            int funds = EncryptedPlayerPrefs.GetInt("Funds");
            int tempFunds = UnityEngine.Random.Range(10, 25);
            funds = tempFunds + funds;
            EncryptedPlayerPrefs.SetInt("Funds", funds);

            totalFundsEarned += tempFunds;

            //if (EncryptedPlayerPrefs.GetString("Highscore", 0) < gameScore)
            //{
            //if (HighscoreManager.Instance)
            //{
            //    EncryptedPlayerPrefs.SetString("Highscore", gameScore);
            //    HighscoreManager.Instance.SetHighscores(gameScore, 1);
            //}
            //}

            //if (fundsEarnedSuccessText)
            //{
            //    fundsEarnedSuccessText.text = totalFundsEarned.ToString();
            //}
            //else
            //    Utility.ErrorLog("fundsEarnedSuccessText is not assigned in IngameUI.cs", 1);

        }
        else
        {
            distanceCoveredFailText.text = ((int)TimeManager.totalDistanceCovered).ToString() + " m";
            timeTakenFailText.text = TimeManager.Instance.GetTimeInFormat();

            int totalGameScore = GetScore((int)TimeManager.totalTimePassed);

            scoreFailText.text = totalGameScore.ToString();

            totalFundsEarned = (int)TimeManager.totalTimePassed + totalScoreAddedFromEnv + (Random.Range(20, 30));

            fundsEarnedFailText.text = totalFundsEarned.ToString();

            ulong gameHighScore = ulong.Parse(EncryptedPlayerPrefs.GetString("Highscore"));
            ulong gameHighScoreWeekly = ulong.Parse(EncryptedPlayerPrefs.GetString("HighScoreWeekly"));
            //ulong gameHighScore = 0;
            //ulong gameHighScoreWeekly = 0;


            //gameHighScore += gameScore;
            //gameHighScoreWeekly += gameScore;
            if (gameScore > gameHighScore)
            {
                EncryptedPlayerPrefs.SetString("Highscore", gameHighScore.ToString());
            }
            if (gameScore > gameHighScoreWeekly)
            {
                EncryptedPlayerPrefs.SetString("HighScoreWeekly", gameHighScoreWeekly.ToString());
            }

            int playerLoadoutNumber = 3;
            playerLoadoutNumber = GameManager.Instance.GetLoadoutInfo(playerLoadoutNumber);

            if (GameServerData.Instance)
                GameServerData.Instance.SetData(gameHighScore.ToString(), gameHighScoreWeekly.ToString(), playerLoadoutNumber);


            LevelCompleteMusic.Instance.PlayFailMusic();

        }
    }
    int GetScore(int timeScore)
    {
        int totalGameScore = timeScore + totalScoreAddedFromEnv;
        totalGameScore = totalGameScore * 67;
        return totalGameScore;
    }
    public void OpenItemStorePanel()
    {
            if (itemStorePanel)
            {
                if (itemStorePanel.GetComponent<ItemShopManager>())
                {
                    itemStorePanel.SetActive(true);
                }
                else
                    Utility.ErrorLog("ItemShopManager Component not found on Loadouts Panel", 2);
            }
            else
                Utility.ErrorLog("Item Shop Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void OpenItemsPanel()
    {
        if (itemsPanel)
        {
            itemsPanel.SetActive(true);
            MainMenuCloseOperations();
        }
        else
                Utility.ErrorLog("InsufficientFundsManager Component not found on Insufficient Funds Panel", 2);

        Utility.MakeClickSound();
    }

    public void ClosePrizeRedeemPanel()
    {
        MainMenuOpenOperations();
        if (StartPanel)
        {
            StartPanel.SetActive(true);
        }
        prizeShowPanel.SetActive(false);
        Utility.MakeClickSound();
    }

    public void CloseItemsPanel()
    {
        if (itemsPanel)
        {
            itemsPanel.SetActive(false);
            StartPanel.SetActive(true);
            MainMenuOpenOperations();
        }
        else
            Utility.ErrorLog("InsufficientFundsManager Component not found on Insufficient Funds Panel", 2);

        Utility.MakeClickSound();
    }

    public void OpenInappPanel()
    {
        if (InsufficientFundsPanel)
        {
            if (InsufficientFundsPanel)
            {
                if (InsufficientFundsPanel.GetComponent<InsufficientFundsManager>())
                {
                    InsufficientFundsPanel.GetComponent<InsufficientFundsManager>().OpenDirectInapp = true;
                    InsufficientFundsPanel.SetActive(true);
                }
                else
                    Utility.ErrorLog("InsufficientFundsManager Component not found on Insufficient Funds Panel", 2);
            }
            else
                Utility.ErrorLog("Insufficient Funds Panel is not assigned in MainMenuUI.cs", 1);
        }
        else
            Utility.ErrorLog("Insufficient Funds Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void RemoveAds()
    {
        //if (AdManager.Instance)
        //{
        //    GameObject adManager = AdManager.Instance.gameObject;
        //    if (adManager.GetComponent("Purchaser"))
        //    {
        //        adManager.GetComponent("Purchaser").SendMessage("RemoveAdsPurchaseCompleted");
        //    }
        //    else
        //        Utility.ErrorLog("Purchaser is not found in IngameUI.cs " + " of " + this.gameObject.name, 2);
        //}
        //Utility.MakeClickSound();
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
            _musicSource.PlayMainMenuMusic();
        }
        else
            Utility.ErrorLog("Music Source is not assigned in IngameUI.cs", 1);
    }
    public static void PlayMusicIngame()
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
        else if (GameManager.Instance.vibrationValue == 0)
        {
            //Vibration.Vibrate(50);
            GameManager.Instance.vibrationValue = 1;
            EncryptedPlayerPrefs.SetInt("vibrationValue", 1);
        }
        //ChangeVibrationSprite();
    }

    //public void ChangeVibrationSprite()
    //{
    //    if (EncryptedPlayerPrefs.GetInt("vibrationValue") == 1)
    //    {
    //        vibrationObj.sprite = vibrationOn;
    //    }
    //    else
    //   if (EncryptedPlayerPrefs.GetInt("vibrationValue") == 0)
    //    {
    //        vibrationObj.sprite = vibrationOff;
    //    }
    //    //Utility.MakeClickSound();
    //}

    public void SetGDRP()
    {
        splashScreenPanel.SetActive(false);
        languagesPanel.SetActive(false);
        if (EncryptedPlayerPrefs.GetInt("Consent") == 0)
        {
            if (GDRPConsentPanel)
                GDRPConsentPanel.SetActive(true);
            else
                Utility.ErrorLog("Consent Panel is not assigned in MainMenuUI.cs", 1);
        }
        else
        {
            GDRPConsentTakenAlready();
        }
    }
    public void SetGDRPConsent(bool consentValue)
    {
        EncryptedPlayerPrefs.SetInt("Consent", 1);
        if (AdManager.Instance)
        {
            //AdManager.Instance.SetGDRPConsent(consentValue);//aliedit
        }
        if (GDRPConsentPanel)
            GDRPConsentPanel.SetActive(false);
        else
            Utility.ErrorLog("GDRP Consent Panel is not assigned in MainMenuUI.cs", 1);

        StartOfMainGame();
        Utility.MakeClickSound();
    }
    public void GDRPConsentTakenAlready()
    {
        if (AdManager.Instance)
        {
            //AdManager.Instance.GDRPConsentTakenAlready();//aliedit
        }
        StartOfMainGame();
        Utility.MakeClickSound();
    }

    void StartOfMainGame()
    {
        MainMenuOpenOperations();
        if (splashScreenPanel)
            splashScreenPanel.SetActive(false);
        if (StartPanel)
            StartPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        CheckForFortuneWheel();
    }
    public void TakePlayerUsernameInput()
    {
        if (EncryptedPlayerPrefs.GetString("PlayerName") == "")
        {
            Debug.Log("---2---");
            playerInputPanel.SetActive(true);
        }
        else
        {
            playerInputPanel.SetActive(false);
            CheckForLanguages();
        }
    }

    public void OpenFortuneWheelPanel()
    {
        MainMenuCloseOperations();
        fortuneWheelPanel.SetActive(true);
        StartPanel.SetActive(false);
        Utility.MakeClickSound();
    }
    public void CloseFortuneWheelPanel()
    {
        fortuneWheelPanel.SetActive(false);
        MainMenuOpenOperations();
        StartPanel.SetActive(true);
        Utility.MakeClickSound();
    }

    public void MainMenuOpenOperations()
    {
        ShowBannerIfNotBeingShownAlready();
        CheckForFortuneWheel();
        dailyRewardPanel.SetActive(true);
        TurnParticlesOn();
    }
    public void MainMenuCloseOperations()
    {
        dailyRewardPanel.SetActive(false);
        TurnParticlesOff();
    }
    public void ActivateMainMenuPanel()
    {
        MainMenuOpenOperations();
        if (splashScreenPanel)
            splashScreenPanel.SetActive(false);
        if (StartPanel)
            StartPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        CheckForFortuneWheel();
    }

    void CheckForFortuneWheel()
    {
        bool checkFortuneWheelTime = FortuneWheelManager.CheckTime();
        //Debug.Log("checkFortuneWheelTime " + checkFortuneWheelTime);
        if (checkFortuneWheelTime)
        {
            //EncryptedPlayerPrefs.SetInt("FortuneWheelPressedTimes", 0);
            fortuneWheelMarkBlinker.SetActive(true);
        }
        else
        {
            fortuneWheelMarkBlinker.SetActive(false);
        }
    }

    public void TurnParticlesOn()
    {
        //foreach (var item in menuParticles)
        //{
        //    item.SetActive(true);
        //}
    }
    public void TurnParticlesOff()
    {
        //foreach (var item in menuParticles)
        //{
        //    item.SetActive(false);
        //}
    }

    public void AdjustLowVolume()
    {
        musicSource.GetComponent<SoundAndMusic>().MusicLowAdjust(musicLowValue);
    }

    public void AdjustHighVolume()
    {
        musicSource.GetComponent<SoundAndMusic>().MusicHighAdjust();
    }

    public void RedeemButtonClicked()
    {
        MainMenuCloseOperations();
        GameServerData.Instance.CheckPricesOfCryptocurrency();
        Utility.MakeClickSound();

        if (EncryptedPlayerPrefs.GetInt("playerGuestNameSet") == 1 || EncryptedPlayerPrefs.GetString("PlayerName") == "GuestUser")
        {
            playerInputPanel.SetActive(true);
        }
    }

    public void SetYearWeek(int yearWeek)
    {
        if (EncryptedPlayerPrefs.GetInt("YearWeek") < yearWeek)
        {
            EncryptedPlayerPrefs.SetString("HighScoreWeekly", "0");
            EncryptedPlayerPrefs.SetInt("YearWeek", yearWeek);
            GameServerData.Instance.UpdateYearWeek(yearWeek);
        }
    }

    public void SetEncryptedPlayerPrefs()
    {
        if (!EncryptedPlayerPrefs.HasKey("YearWeek"))
        {
            EncryptedPlayerPrefs.SetInt("YearWeek", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("FortuneWheelPressedTimes"))
        {
            EncryptedPlayerPrefs.SetInt("FortuneWheelPressedTimes", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("ReferralPrize"))
        {
            EncryptedPlayerPrefs.SetInt("ReferralPrize", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("TokenCollected"))
        {
            EncryptedPlayerPrefs.SetInt("TokenCollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("BTCCollected"))
        {
            EncryptedPlayerPrefs.SetInt("BTCCollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("ETHCollected"))
        {
            EncryptedPlayerPrefs.SetInt("ETHCollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("BNBCollected"))
        {
            EncryptedPlayerPrefs.SetInt("BNBCollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("USDTCollected"))
        {
            EncryptedPlayerPrefs.SetInt("USDTCollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("XRPCollected"))
        {
            EncryptedPlayerPrefs.SetInt("XRPCollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("DOGECollected"))
        {
            EncryptedPlayerPrefs.SetInt("DOGECollected", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("LanguageSelected"))
        {
            EncryptedPlayerPrefs.SetInt("LanguageSelected", -1);
        }
        if (!EncryptedPlayerPrefs.HasKey("playerGuestNameSet"))
        {
            EncryptedPlayerPrefs.SetInt("playerGuestNameSet", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("playerNameSet"))
        {
            EncryptedPlayerPrefs.SetInt("playerNameSet", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("PlayerName"))
        {
            EncryptedPlayerPrefs.SetString("PlayerName", "");
        }
        if (!EncryptedPlayerPrefs.HasKey("PlayerGuestName"))
        {
            EncryptedPlayerPrefs.SetString("PlayerGuestName", "");
        }
        if (!EncryptedPlayerPrefs.HasKey("country"))
        {
            EncryptedPlayerPrefs.SetString("country", "");
        }
        if (!EncryptedPlayerPrefs.HasKey("city"))
        {
            EncryptedPlayerPrefs.SetString("city", "");
        }
        if (!EncryptedPlayerPrefs.HasKey("Highscore"))
        {
            EncryptedPlayerPrefs.SetString("Highscore", "0");
        }
        if (!EncryptedPlayerPrefs.HasKey("HighScoreWeekly"))
        {
            EncryptedPlayerPrefs.SetString("HighScoreWeekly", "0");
        }
        if (!EncryptedPlayerPrefs.HasKey("TutorialFinished"))
        {
            EncryptedPlayerPrefs.SetInt("TutorialFinished", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("Tutorial"))
        {
            EncryptedPlayerPrefs.SetInt("Tutorial", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("Funds"))
        {
            EncryptedPlayerPrefs.SetInt("Funds", 10);
        }
        if (!EncryptedPlayerPrefs.HasKey("ReleaseDate"))
        {
            EncryptedPlayerPrefs.SetInt("ReleaseDate", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("Consent"))
        {
            EncryptedPlayerPrefs.SetInt("Consent", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("vibrationValue"))
        {
            EncryptedPlayerPrefs.SetInt("vibrationValue", 1);
        }
        if (!EncryptedPlayerPrefs.HasKey("SensitivityValue"))
        {
            EncryptedPlayerPrefs.SetFloat("SensitivityValue", 0.5f);
            GameManager.Instance.sensitivityValue = 0.5f;
        }
        if (!PlayerPrefs.HasKey("MusicValue"))
        {
            PlayerPrefs.SetFloat("MusicValue", 0.75f);
            GameManager.Instance.musicValue = 0.75f;
        }
        if (!PlayerPrefs.HasKey("SoundValue"))
        {
            PlayerPrefs.SetFloat("SoundValue", 1f);
            GameManager.Instance.soundValue = 1f;
        }
        if (!EncryptedPlayerPrefs.HasKey("LevelsUnocked"))
        {
            EncryptedPlayerPrefs.SetInt("LevelsUnocked", 1);
        }
        if (!EncryptedPlayerPrefs.HasKey("FirstTimeEncryptedPlayerPrefsSettings"))
        {
            Utility.ShowHeaderValues();
        }
        if (itemsPanel)
        {
            if (itemsPanel.GetComponent<LoadoutManager>())
            {
                LoadoutManager managerScript = itemsPanel.GetComponent<LoadoutManager>();
                if (!EncryptedPlayerPrefs.HasKey("WeaponsEncryptedPlayerPrefsSet"))
                {
                    EncryptedPlayerPrefs.SetInt("WeaponsEncryptedPlayerPrefsSet", 1);

                    for (int i = 1; i < managerScript.loadouts.Length; i++)
                    {
                        string prefKey = "";
                        int prefValue = 0;
                        if (i < managerScript.loadouts.Length)
                        {
                            for (int j = 1; j < managerScript.loadouts[i].weapon.Length; j++)
                            {
                                if (j < managerScript.loadouts[i].weapon.Length)
                                {
                                    prefKey = "Loadout" + i + "Weapon" + j;

                                    if (j == 1 && managerScript.loadouts[i].firstEnabledByDefault)
                                    {
                                        prefValue = j;
                                        string equipKey = "Loadout" + i + "Equipped";
                                        EncryptedPlayerPrefs.SetInt(equipKey, j);
                                        Utility.SimpleLog("Setting Equipped pref: " + equipKey + " and its value is " + j);
                                    }
                                    else
                                    {
                                        prefValue = 0;
                                        if (!managerScript.loadouts[i].firstEnabledByDefault)
                                        {
                                            string equipKey = "Loadout" + i + "Equipped";
                                            EncryptedPlayerPrefs.SetInt(equipKey, prefValue);
                                            Utility.SimpleLog("Setting Equipped pref: " + equipKey + " and its value is " + prefValue);
                                        }
                                    }

                                    Utility.SimpleLog("Setting weapon pref: " + prefKey + " and its value is " + prefValue);
                                    EncryptedPlayerPrefs.SetInt(prefKey, prefValue);
                                }
                                else
                                    Utility.ErrorLog("Loadout Objects array is out of bound in Loadouts.cs", 4);
                            }
                        }
                        else
                            Utility.ErrorLog("Loadout script array is out of bound in LoadoutManager.cs", 4);
                    }
                }
            }
            else
                Utility.ErrorLog("LoadoutManager Component not found on Items Panel", 2);
        }
        else
            Utility.ErrorLog("Loadouts Panel is not assigned in MainMenuUI.cs", 1);
    }
}