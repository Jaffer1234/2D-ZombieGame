using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MainMenuUI : MonoBehaviour
{
    [Header("Sound And Music")]
    [SerializeField] private SoundManager soundSource;
    [SerializeField] private MusicManager musicSource;
    public static SoundManager _soundSource;
    public static MusicManager _musicSource;

    [Header("Public Menu Panels")]
    public GameObject GDRPConsentPanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    //public GameObject modulesPanel;
    //public GameObject levelsPanel;
    //public GameObject loadoutsPanel;
    public GameObject itemShopPanel;
    public GameObject InsufficientFundsPanel;
    public GameObject loadingPanel;

    public GameObject ModeSelectionPanel;
    //public GameObject tutorialPanel;

    [Header("Vibration Sprites")]
    [SerializeField] private Sprite vibrationOn;
    [SerializeField] private Sprite vibrationOff;
    [SerializeField] private Image vibrationObj;

    public static MainMenuUI Instance;
    private static bool firstTimeLoading = true; 

    [Header("Text Fields")]
    public Text serverConnectionErrorPanelText;

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
        //PlayerPrefs.SetInt("RemoveAds", 0);
        if (!EncryptedPlayerPrefs.HasKey("RemoveAds"))
        {
            EncryptedPlayerPrefs.SetInt("RemoveAds", 0);
        }
        Time.timeScale = 1f;

        ChangeVibrationSprite();

        //if (levelsPanel)
        //{
        //    if (levelsPanel.GetComponent<LevelsLockManager>())
        //    {
        //        GameManager.Instance.levelsInAModule = levelsPanel.GetComponent<LevelsLockManager>().levelsUnlockedObjects.Length - 1;
        //    }
        //    else
        //        Utility.ErrorLog("LevelsLockManager Component not found on Levels Panel", 2);
        //}
        //else
        //    Utility.ErrorLog("Levels Panel is not assigned in MainMenuUI.cs", 1);
        //if (loadoutsPanel)
        //{
        //    if (loadoutsPanel.GetComponent<LoadoutManager>())
        //    {
        //        GameManager.Instance.loadoutCount = loadoutsPanel.GetComponent<LoadoutManager>().loadouts.Length - 1;
        //    }
        //    else
        //        Utility.ErrorLog("LoadoutManager Component not found on Loadouts Panel", 2);
        //}
        //else
        //    Utility.ErrorLog("Loadouts Panel is not assigned in MainMenuUI.cs", 1);

        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);

        if (GDRPConsentPanel)
            GDRPConsentPanel.SetActive(false);

        if (GameManager.Instance.comingFromIngame)
        {
            GameManager.Instance.comingFromIngame = false;

            PlayMusic();
            if (mainMenuPanel)
                mainMenuPanel.SetActive(true);
            else
                Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

            //if (levelsPanel)
            //    levelsPanel.SetActive(true);
            //else
            //    Utility.ErrorLog("Modules Panel is not assigned in MainMenuUI.cs", 1);
        }
        else
        {
            //if (logoSplashScreenPanel)
            //{
            //    logoSplashScreenPanel.SetActive(true);
            //    StartCoroutine(logoScren());
            //}
        }
    }

    //IEnumerator logoScren()
    //{
    //    yield return new WaitForSeconds(3.5f);
    //    logoSplashScreenPanel.SetActive(false);
    //    if (splashScreenPanel)
    //    {
    //        splashScreenPanel.SetActive(true);
    //    }
    //    PlayMusic();
    //}

    public void OpenModeSelectionMode()
    {
        mainMenuPanel.SetActive(false);
        ModeSelectionPanel.SetActive(true);

        Utility.MakeClickSound();
    }

    public void CloseModeSelectionMode()
    {
        mainMenuPanel.SetActive(true);
        ModeSelectionPanel.SetActive(false);

        SoundManager.Instance.PlayUICloseSound();
    }

    public void SetGDRP()
    {
        //splashScreenPanel.SetActive(false);
        if (EncryptedPlayerPrefs.GetInt("Consent") == -1)
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
        EncryptedPlayerPrefs.SetInt("Consent", 0);

        if (GDRPConsentPanel)
            GDRPConsentPanel.SetActive(false);
        else
            Utility.ErrorLog("GDRP Consent Panel is not assigned in MainMenuUI.cs", 1);

        //TakePlayerUsernameInput();

        Utility.MakeClickSound();
    }

    public void GDRPConsentTakenAlready()
    {
        EncryptedPlayerPrefs.SetInt("Consent", 0);
       
        StartOfMainGame();
        //if (AdManager.Instance)
        //{
        //    AdManager.Instance.GDRPConsentTakenAlready();
        //}
        //TakePlayerUsernameInput();

        Utility.MakeClickSound();
    }

    //public void TakePlayerUsernameInput()
    //{
    //    if (EncryptedPlayerPrefs.GetString("PlayerName") == "")
    //    {
    //        playerInputPanel.SetActive(true);
    //    }
    //    else
    //    {
    //        playerInputPanel.SetActive(false);
    //        StartOfMainGame();
    //    }
    //}

    void StartOfMainGame()
    {
        if (firstTimeLoading && (Application.platform != RuntimePlatform.WindowsEditor || !EncryptedPlayerPrefs.HasKey("FirstTimeEncryptedPlayerPrefsSettings")))
        {
            EncryptedPlayerPrefs.SetInt("FirstTimeEncryptedPlayerPrefsSettings", 1);

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
        //HighscoreManager.Instance.GetHighscores();
        //HighscoreManager.Instance.SetPreviousPanel(mainMenuPanel);
    }

    //public void CloseErrorPanel()
    //{
    //    //GameObject prevPanel = HighscoreManager.Instance.GetPreviousPanel();
    //    //if (prevPanel != null)
    //    //{
    //    //    prevPanel.SetActive(true);
    //    //}
    //    serverConnectionErrorPanel.SetActive(false);
    //}

    public void PlayGamePressed(int mode)
    {
        GameManager.Instance.moduleNumber = mode;
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

    public void ActivateMainMenuPanel()
    {
        //if (loadoutsPanel)
        //    loadoutsPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Loadouts Panel is not assigned in MainMenuUI.cs", 1);

        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);
    }

    public void PlayStart()
    {
        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        //if (modulesPanel)
        //    modulesPanel.SetActive(true);
        //else
        //    Utility.ErrorLog("Modules Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void BackFromModules()
    {
        //if (modulesPanel)
        //    modulesPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Modules Panel is not assigned in MainMenuUI.cs", 1);

        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);
        
        SoundManager.Instance.PlayUICloseSound();
    }

    public void ModuleSelected(int moduleNo)
    {
        GameManager.Instance.moduleNumber = moduleNo;

        //if (modulesPanel)
        //    modulesPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Module Panel is not assigned in MainMenuUI.cs", 1);

        //if (levelsPanel)
        //    levelsPanel.SetActive(true);
        //else
        //    Utility.ErrorLog("Levels Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void LevelSelected(int levelNo)
    {
        //if (levelNo == 0) levelNo = 1;
        GameManager.Instance.levelNumber = levelNo;//= (GameManager.Instance.moduleNumber * GameManager.Instance.levelsInAModule) + levelNo;

        //if (levelsPanel)
        //    levelsPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Levels Panel is not assigned in MainMenuUI.cs", 1);

        StartLoading();
    }

    public void BackFromLevels()
    {
        //if (levelsPanel)
        //    levelsPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Levels Panel is not assigned in MainMenuUI.cs", 1);

        //if (modulesPanel)
        //    modulesPanel.SetActive(true);
        //else
        //    Utility.ErrorLog("Module Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void OpenSettingPanel()
    {
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

        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        SoundManager.Instance.PlayUICloseSound();
    }

    public void OpenLoadoutsPanel()
    {
        if (mainMenuPanel)
            mainMenuPanel.SetActive(false);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

        //if (loadoutsPanel)
        //    loadoutsPanel.SetActive(true);
        //else
        //    Utility.ErrorLog("Loadouts Panel is not assigned in MainMenuUI.cs", 1);

        Utility.MakeClickSound();
    }

    public void BackFromLoadouts()
    {
        //if (loadoutsPanel)
        //    loadoutsPanel.SetActive(false);
        //else
        //    Utility.ErrorLog("Loadouts Panel is not assigned in MainMenuUI.cs", 1);

        if (mainMenuPanel)
            mainMenuPanel.SetActive(true);
        else
            Utility.ErrorLog("Main Menu Panel is not assigned in MainMenuUI.cs", 1);

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
            } else
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
            GameManager.Instance.vibrationValue = 0;
            EncryptedPlayerPrefs.SetInt("vibrationValue", 0);
        }
        else if (GameManager.Instance.vibrationValue == 0)
        {
            //Vibration.Vibrate(50);

            GameManager.Instance.vibrationValue = 1;
            EncryptedPlayerPrefs.SetInt("vibrationValue", 1);
        }

        ChangeVibrationSprite();
    }

    public void ChangeVibrationSprite()
    {
        if (EncryptedPlayerPrefs.GetInt("vibrationValue") == 1)
        {
            vibrationObj.sprite = vibrationOn;
        }
        else
       if (EncryptedPlayerPrefs.GetInt("vibrationValue") == 0)
        {
            vibrationObj.sprite = vibrationOff;
        }

        //Utility.MakeClickSound();
    }

    public void SetEncryptedPlayerPrefs()
    {
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
        if (!EncryptedPlayerPrefs.HasKey("PlayerName"))
        {
            EncryptedPlayerPrefs.SetString("PlayerName", "");
        }
        if (!EncryptedPlayerPrefs.HasKey("Highscore"))
        {
            EncryptedPlayerPrefs.SetString("Highscore", "0");
        }
        if (!EncryptedPlayerPrefs.HasKey("ReleaseDate"))
        {
            EncryptedPlayerPrefs.SetInt("ReleaseDate", 0);
        }
        if (!EncryptedPlayerPrefs.HasKey("Consent"))
        {
            EncryptedPlayerPrefs.SetInt("Consent", -1);
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
        if (!EncryptedPlayerPrefs.HasKey("FirstTimeEncryptedPlayerPrefsSettings"))
        {
            Utility.ShowHeaderValues();
        }
    }
}