using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour, IConfirmation
{

    public ConfirmationWindow confirmationWindow;

    public List<GameObject> panels = new List<GameObject>();

    public Toggle ExitButton;

    public GameObject FPSPanel;

    // indeces of panels
    // 0 == Play
    // 1 == Hiscore
    // 2 == Settings
    // 3 == Credits

    GameObject currentPanel = null;

    public AudioClip mainMenuMusicAudioClip;

    private void Start()
    {
        // load all settings from playerPrefs
        LoadSettings();
        
        if(mainMenuMusicAudioClip != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuMusicAudioClip);
        }
    }

    // this is called from unity button in the left panel in the main menu
    public void ShowPanel(GameObject go)
    {
        currentPanel = (currentPanel == go) ? null : go;
        foreach(GameObject panel in panels)
        {
            panel.SetActive(panel == currentPanel);
        }
    }

    public void ExitButtonClicked()
    {
        confirmationWindow.Init(this, "ExitButton", "You are about to exit to Windows, are You sure?");
        confirmationWindow.gameObject.SetActive(true);
        ExitButton.isOn = false;
        ExitButton.gameObject.GetComponent<MenuCustomToggleButton>().UpdateValue();
    }

    public void ConfirmationSucceeded(string whoAddressed)
    {
        if (whoAddressed.Equals("ExitButton"))
        {
#if UNITY_EDITOR
            Debug.Log("ExitButtonClicked, real exiting is blocked in editor");
            confirmationWindow.gameObject.SetActive(false);
# else
            Application.Quit();
#endif
        }
    }

    public void ConfirmationCanceled(string whoAddressed)
    {
        if (whoAddressed.Equals("ExitButton"))
        {
            Debug.Log("Exiting cancelled");
            confirmationWindow.gameObject.SetActive(false);
        }
    }

    private void LoadSettings()
    {
        // go through all the options such as anti aliasing, showFPS...

        // actual settings

        // graphics
        //UIRESOLUTION


        // FULLSCREEN
        Screen.fullScreen = PlayerPrefs.GetInt(ISettings.Type.FULLSCREEN.ToString(), 1) == 1;

        // REFRESHRATE
        int frameRateIndex = PlayerPrefs.GetInt(ISettings.Type.REFRESHRATE.ToString(), 2);
        Application.targetFrameRate = frameRateIndex == 0 ? 30 : frameRateIndex == 1 ? 60 : -1;

        // SHOWFPS
        FPSPanel.SetActive((PlayerPrefs.GetInt(ISettings.Type.SHOWFPS.ToString(), 0) == 1));

        // WINDOWMODE
        FullScreenMode[] fullScreenModes = new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.Windowed };
        Screen.fullScreenMode = fullScreenModes[PlayerPrefs.GetInt(ISettings.Type.WINDOWMODE.ToString(), System.Array.IndexOf(fullScreenModes, Screen.fullScreenMode))];

        // VSYNC
        QualitySettings.vSyncCount = PlayerPrefs.GetInt(ISettings.Type.VERTICALSYNC.ToString(), 1);

        // ANTIALIASING
        int[] intArray = new int[] { 0, 2, 4, 8 };
        QualitySettings.antiAliasing = intArray[PlayerPrefs.GetInt(ISettings.Type.ANTIALIAS.ToString(), 0)];

        // BRIGHTNESS
        Screen.brightness = PlayerPrefs.GetFloat(ISettings.Type.BRIGHTNESS.ToString(), 1f);

        // BLOOM
        //OnEnableBloom(PlayerPrefs.GetInt(ISettings.Type.BLOOM.ToString()) == 1);

        // LIGHTQUALITY not implemented
        // SHADOWQUALITY not implemented

        // audio
        // MASTERVOLUME loaded in audioManager
        // MUSICVOLUME loaded in audioManager
        // SOUNDEFFECTVOLUME loaded in audioManager
        // UIVOLUME loaded in audioManager
        // MUTEMUSICONPAUSE loaded in PauseMenuScript when player pauses

        // gameplay
        //ShowPlayerHealthBar(PlayerPrefs.GetInt(ISettings.Type.SHOWPLAYERHEALTHBAR.ToString(), 1) == 1);
        //ShowEnemyHealthBars(PlayerPrefs.GetInt(ISettings.Type.SHOWENEMYHEALTHBAR.ToString(), 1) == 1);
    }
}
