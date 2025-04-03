using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerBaseHealthBar playerBaseHealthBar;
    public BloomActivator bloomActivator;

    public GameObject FPSPanel;

    public delegate void ShowEnemyHealthBar(bool show);
    public static event ShowEnemyHealthBar OnShowEnemyHealthBar;

    public delegate void EnablePostProcessingBloom(bool enable);
    public static event EnablePostProcessingBloom OnEnableBloom;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // this is called when scene loads


        //TODO: go through all the options such as anti aliasing, showFPS...

        // actual settings

        // graphics
        //UIRESOLUTION


        // FULLSCREEN
        Screen.fullScreen = PlayerPrefs.GetInt(ISettings.Type.FULLSCREEN.ToString(), 0) == 1;
        
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
        OnEnableBloom(PlayerPrefs.GetInt(ISettings.Type.BLOOM.ToString()) == 1);
        
        // LIGHTQUALITY not implemented
        // SHADOWQUALITY not implemented

        // audio
        // MASTERVOLUME loaded in audioManager
        // MUSICVOLUME loaded in audioManager
        // SOUNDEFFECTVOLUME loaded in audioManager
        // UIVOLUME loaded in audioManager
        // MUTEMUSICONPAUSE loaded in PauseMenuScript when player pauses

        // gameplay
        ShowPlayerHealthBar(PlayerPrefs.GetInt(ISettings.Type.SHOWPLAYERHEALTHBAR.ToString(), 1) == 1);
        ShowEnemyHealthBars(PlayerPrefs.GetInt(ISettings.Type.SHOWENEMYHEALTHBAR.ToString(), 1) == 1);
    }

    public void ShowPlayerHealthBar(bool show)
    {
        playerBaseHealthBar.gameObject.SetActive(show);
    }

    public void ShowEnemyHealthBars(bool show)
    {
        OnShowEnemyHealthBar?.Invoke(show);
    }

    public void EnableBloom(bool enable)
    {
        OnEnableBloom?.Invoke(enable);
    }

    private void OnEnable()
    {
        MenuSettingsPanelScript.OnShowPlayerHealthBar += ShowPlayerHealthBar;
        MenuSettingsPanelScript.OnShowEnemyHealthBar += ShowEnemyHealthBars;
        MenuSettingsPanelScript.OnEnableBloom += EnableBloom;
    }

    private void OnDisable()
    {
        MenuSettingsPanelScript.OnShowPlayerHealthBar -= ShowPlayerHealthBar;
        MenuSettingsPanelScript.OnShowEnemyHealthBar -= ShowEnemyHealthBars;
        MenuSettingsPanelScript.OnEnableBloom -= EnableBloom;
    }
}
