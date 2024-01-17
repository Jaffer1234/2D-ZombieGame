using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("URL")]
    public string iosAppURL;
    public string privacyURL;

    [Header("Particles")]
    public GameObject[] menuParticles;
    [Header("Sound And Music")]
    [SerializeField] private SoundManager soundSource;
    [SerializeField] private MusicManager musicSource;
    public static SoundManager _soundSource;
    public static MusicManager _musicSource;

    [Header("Public Menu Panels")]
    public GameObject[] languageTicks;
    public GameObject languagesPanel;
    public GameObject GDRPConsentPanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    //public GameObject modulesPanel;
    //public GameObject levelsPanel;
    //public GameObject loadoutsPanel;

    public GameObject itemShopPanel;
    public GameObject InsufficientFundsPanel;
    public GameObject loadingPanel;
    public GameObject leadboardPanel;
    public GameObject serverConnectionErrorPanel;
    public GameObject pleaseWaitPanel;

    public GameObject logoSplashScreenPanel;
    public GameObject splashScreenPanel;
    public GameObject playerInputPanel;
    public GameObject ModeSelectionPanel;
    public GameObject prizeShowPanel;
    public GameObject fillInformationForPrizePanel;
    public GameObject fortuneWheelPanel;
    public GameObject dailyRewardPanel;

    //public GameObject tutorialPanel;


    [Header("Vibration Sprites")]
    [SerializeField] private Sprite vibrationOn;
    [SerializeField] private Sprite vibrationOff;
    [SerializeField] private Image vibrationObj;

    public static MainMenuUI Instance;
    private static bool firstTimeLoading = true; 

    [Header("Text Fields")]
    public Text serverConnectionErrorPanelText;


    public GameObject fortuneWheelMarkBlinker;
    public GameObject dailyRewardCanvasPanel;

    public Animator advertiserPanel;

    private bool adPanelOpen = false;

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

        ChangeVibrationSprite();

        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);

        if (GDRPConsentPanel)
            GDRPConsentPanel.SetActive(false);

        if (GameManager.Instance.comingFromIngame)
        {
            CheckForFortuneWheel();
            GameManager.Instance.comingFromIngame = false;

            MainMenuOpenOperations();
            PlayMusic();
            if (mainMenuPanel)
            {
                mainMenuPanel.SetActive(true);
               // StartCoroutine(testINST());
            }
            else
                Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);
        }
        else
        {
            if (logoSplashScreenPanel)
            {
                logoSplashScreenPanel.SetActive(true);
                StartCoroutine(logoScren());
            }
        }
        foreach (var item in languageTicks)
        {
            item.SetActive(false);
        }
        if (EncryptedPlayerPrefs.GetInt("LanguageSelected") != -1)
        {
            languageTicks[EncryptedPlayerPrefs.GetInt("LanguageSelected")].SetActive(true);
        }
    }

    //IEnumerator testINST()
    //{
    //    yield return new WaitForSeconds(5f);
    //    AdManager.Instance.ShowInterstitialAd();
    //}

    IEnumerator logoScren()
    {
        yield return new WaitForSeconds(3.5f);
        logoSplashScreenPanel.SetActive(false);

        if (EncryptedPlayerPrefs.GetInt("playerNameSet") == 0 && EncryptedPlayerPrefs.GetInt("playerGuestNameSet") == 0)
        {
            splashScreenPanel.SetActive(true);
        }
        else
        {
            splashScreenPanel.SetActive(false);
            CheckForLanguages();
        }

        PlayMusic();
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
        mainMenuPanel.SetActive(false);
        Utility.MakeClickSound();
    }

    public void OpenModeSelectionMode()
    {
        MainMenuCloseOperations();
        mainMenuPanel.SetActive(false);
        ModeSelectionPanel.SetActive(true);

        Utility.MakeClickSound();
    }

    public void CloseModeSelectionMode()
    {
        MainMenuOpenOperations();
        mainMenuPanel.SetActive(true);
        ModeSelectionPanel.SetActive(false);

        SoundManager.Instance.PlayUICloseSound();
    }

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
        if (GDRPConsentPanel)
            GDRPConsentPanel.SetActive(false);
        else
            Utility.ErrorLog("GDRP Consent Panel is not assigned in MainMenuUI.cs", 1);

        StartOfMainGame();
        Utility.MakeClickSound();
    }

    public void GDRPConsentTakenAlready()
    {
        StartOfMainGame();
        Utility.MakeClickSound();
    }

    public void TakePlayerUsernameInput()
    {
        if (EncryptedPlayerPrefs.GetString("PlayerName") == "")
        {
            playerInputPanel.SetActive(true);
        }
        else
        {
            playerInputPanel.SetActive(false);
            CheckForLanguages();
        }
    }

    void CheckForFortuneWheel()
    {
        bool checkFortuneWheelTime = FortuneWheelManager.CheckTime();
        //Debug.Log("checkFortuneWheelTime " + checkFortuneWheelTime);
        if (checkFortuneWheelTime)
        {
            EncryptedPlayerPrefs.SetInt("FortuneWheelPressedTimes", 0);
            fortuneWheelMarkBlinker.SetActive(true);
        }
        else
        {
            fortuneWheelMarkBlinker.SetActive(false);
        }
    }
    //public void CheckForGiftsIndicatorFunction()
    //{
    //    StartCoroutine(CheckForGiftsIndicator());
    //}
    //IEnumerator CheckForGiftsIndicator()
    //{
    //    yield return null;
    //    yield return null;
    //    yield return null;

    //    bool value = false;
    //    foreach (var item in rewardObjsInd)
    //    {
    //        bool val = item.activeInHierarchy;
    //        value = value || val;
    //    }
    //    rewardObjIndicator.SetActive(value);
    //}
    public void CancelInputPanel()
    {
        //splashScreenPanel.SetActive(true);
        playerInputPanel.SetActive(false);
        Utility.MakeClickSound();
    }

    void StartOfMainGame()
    {
        if (firstTimeLoading && (Application.platform != RuntimePlatform.WindowsEditor || !EncryptedPlayerPrefs.HasKey("FirstTimeEncryptedPlayerPrefsSettings")))
        {
            EncryptedPlayerPrefs.SetInt("FirstTimeEncryptedPlayerPrefsSettings", 1);

            splashScreenPanel.SetActive(false);
            firstTimeLoading = false;
            if (loadingPanel)
            {
                loadingPanel.SetActive(true);

                if (loadingPanel.GetComponent<LoadScene>())
                {
                    loadingPanel.GetComponent<LoadScene>().StartFakeLoading();
                }
                else
                    Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
            }
            else
                Utility.ErrorLog("Loading Panel is not assigned in MainMenuUI.cs", 1);
        }
        else
        {
            ActivateMainMenuPanel();
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

    public void TurnParticlesOn()
    {
        foreach (var item in menuParticles)
        {
            item.SetActive(true);
        }
    }

    public void TurnParticlesOff()
    {
        foreach (var item in menuParticles)
        {
            item.SetActive(false);
        }
    }

    public void PlayGamePressed(int mode)
    {
        GameManager.Instance.moduleNumber = mode;
        MainMenuCloseOperations();
        mainMenuPanel.SetActive(false);
        ModeSelectionPanel.SetActive(false);

        if (EncryptedPlayerPrefs.GetInt("TutorialFinished") == 0)
        {
            TutorialManager.isTutorialRunning = true; 
            LevelSelected(0);
            SoundManager.Instance.PlayPlaySound();
            return;
        }
        
        LevelSelected(EncryptedPlayerPrefs.GetInt("LevelsUnocked"));
        SoundManager.Instance.PlayPlaySound();
    }

    public void MainMenuOpenOperations()
    {
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
        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        CheckForFortuneWheel();
    }

    public void PlayStart()
    {
        MainMenuCloseOperations();
        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void BackFromModules()
    {
        //if (modulesPanel)
        //    modulesPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Modules Panel is not assigned in MainMenuUI.cs", 1);
        MainMenuOpenOperations();
        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);
        
        SoundManager.Instance.PlayUICloseSound();
    }

    public void ModuleSelected(int moduleNo)
    {
        GameManager.Instance.moduleNumber = moduleNo;

        Utility.MakeClickSound();
    }

    public void LevelSelected(int levelNo)
    {
        //if (levelNo == 0) levelNo = 1;

        GameManager.Instance.levelNumber = levelNo;
        
        //= (GameManager.Instance.moduleNumber * GameManager.Instance.levelsInAModule) + levelNo;
        //if (levelsPanel)
        //    levelsPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Levels Panel is not assigned in MainMenuUI.cs", 1);
        StartLoading();
    }

    public void BackFromLevels()
    {
        Utility.MakeClickSound();
    }

    public void OpenSettingPanel()
    {
        MainMenuCloseOperations();
        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        if (settingsPanel)
            settingsPanel.SetActive(true);
        else
            Utility.ErrorLog("Settings Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void BackFromSettings()
    {
        if (settingsPanel)
            settingsPanel.SetActive(false);
        else
            Utility.ErrorLog("Settings Panel is not assigned in MainMenuUI.cs", 1);
        MainMenuOpenOperations();
        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        SoundManager.Instance.PlayUICloseSound();
    }

    public void OpenLoadoutsPanel()
    {
        MainMenuCloseOperations();
        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);


        Utility.MakeClickSound();
    }
    public void BackFromLoadouts()
    {
        MainMenuOpenOperations();
        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void OpenFortuneWheelPanel()
    {
        MainMenuCloseOperations();
        fortuneWheelPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        Utility.MakeClickSound();
    }
    public void CloseFortuneWheelPanel()
    {
        fortuneWheelPanel.SetActive(false);
        MainMenuOpenOperations();
        mainMenuPanel.SetActive(true);
        Utility.MakeClickSound();
    }

    public void StartLoading()
    {
        if (loadingPanel)
        {
            loadingPanel.SetActive(true);

            if (GameManager.Instance.moduleNumber == 0)
            {
                if (loadingPanel.GetComponent<LoadScene>())
                {
                    loadingPanel.GetComponent<LoadScene>().StartLoadingScene("Ingame");
                }
                else
                    Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
            } 
            else
            if (GameManager.Instance.moduleNumber == 1)
            {
                if (loadingPanel.GetComponent<LoadScene>())
                {
                    loadingPanel.GetComponent<LoadScene>().StartLoadingScene("Shooting");
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                }
                else
                    Utility.ErrorLog("LoadScene Component not found on Loading Panel", 2);
            }
        }
        else
            Utility.ErrorLog("Loading Panel Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void OpenItemShopPanel()
    {
        if (itemShopPanel)
        {
            if (itemShopPanel)
            {
                if (itemShopPanel.GetComponent<ItemShopManager>())
                {
                    MainMenuCloseOperations();
                    mainMenuPanel.SetActive(false);
                    itemShopPanel.GetComponent<ItemShopManager>().previousPanelToOpen = mainMenuPanel.gameObject;
                    itemShopPanel.SetActive(true);
                }
                else
                    Utility.ErrorLog("ItemShopManager Component not found on Loadouts Panel", 2);
            }
            else
                Utility.ErrorLog("Item Shop Panel is not assigned in MainMenuUI.cs", 1);
        }
        else
            Utility.ErrorLog("Item Shop Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void OpenInappPanel()
    {
        MainMenuCloseOperations();
        if (mainMenuPanel)
        {
            mainMenuPanel.SetActive(false);
            InsufficientFundsPanel.GetComponent<InsufficientFundsManager>().cameFromMainMenu = true;
        }

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

    public void ClosePrizeRedeemPanel()
    {
        MainMenuOpenOperations();
        if (mainMenuPanel)
        {
            mainMenuPanel.SetActive(true);
        }
        prizeShowPanel.SetActive(false);
        Utility.MakeClickSound();
    }

    public void CloseFillInformationPanel()
    {
        prizeShowPanel.SetActive(true);
        fillInformationForPrizePanel.SetActive(false);
        Utility.MakeClickSound();
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

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(privacyURL);
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
                Utility.ErrorLog("Purchaser is not found in MainMenuUI.cs " + " of " + this.gameObject.name, 2);
        }

        Utility.MakeClickSound();
    }

    public void RateUs()
    {
        string url = "https://play.google.com/store/apps/details?id=" + Application.identifier;
        Application.OpenURL(url);
    }

    public void MakeClickSound()
    {
        if (_soundSource)
        {
            _soundSource.PlayUIClickedSound();
        }
        else
            Utility.ErrorLog("Sound Source is not assigned in MainMenuUI.cs", 1);
    }

    public static void PlayMusic()
    {
        if (_musicSource)
        {
           _musicSource.PlayMainMenuMusic();
        }
        else
            Utility.ErrorLog("Music Source is not assigned in MainMenuUI.cs", 1);
    }

    public void ShowInterstitialAd(bool dateCheckDependency)
    {
        if (AdManager.Instance)
        {
            AdManager.Instance.ShowInterstitialAd();
        }
    }

    public void ControlViberation()
    {
        if (GameManager.Instance.vibrationValue == 1)
        {
            EncryptedPlayerPrefs.SetInt("vibrationValue", 0);
        }
        else
        if (GameManager.Instance.vibrationValue == 0)
        {
            EncryptedPlayerPrefs.SetInt("vibrationValue", 1);
        }

        ChangeVibrationSprite();
        Utility.MakeClickSound();
    }

    public void ChangeVibrationSprite()
    {
        if (EncryptedPlayerPrefs.GetInt("vibrationValue") == 1)
        {
            GameManager.Instance.vibrationValue = 1;
            vibrationObj.sprite = vibrationOn;
        }
        else
        if (EncryptedPlayerPrefs.GetInt("vibrationValue") == 0)
        {
            GameManager.Instance.vibrationValue = 0;
            vibrationObj.sprite = vibrationOff;
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
            PlayerPrefs.SetFloat("MusicValue", 0.5f);
            GameManager.Instance.musicValue = 0.5f;
        }
        if (!PlayerPrefs.HasKey("SoundValue"))
        {
            PlayerPrefs.SetFloat("SoundValue", 0.5f);
            GameManager.Instance.soundValue = 0.5f;
        }
        if (!EncryptedPlayerPrefs.HasKey("LevelsUnocked"))
        {
            EncryptedPlayerPrefs.SetInt("LevelsUnocked", 1);
        }
        if (!EncryptedPlayerPrefs.HasKey("LineBomb"))
        {
            EncryptedPlayerPrefs.SetInt("LineBomb", 3);
        }
        if (!EncryptedPlayerPrefs.HasKey("RadiusBomb"))
        {
            EncryptedPlayerPrefs.SetInt("RadiusBomb", 2);
        }
        if (!EncryptedPlayerPrefs.HasKey("TimeBomb"))
        {
            EncryptedPlayerPrefs.SetInt("TimeBomb", 2);
        }
        if (!EncryptedPlayerPrefs.HasKey("FirstTimeEncryptedPlayerPrefsSettings"))
        {
            Utility.ShowHeaderValues();
        }
    }
}