using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MenuSettingsPanelScript : MonoBehaviour, ISettings
{
    public List<Toggle> toggles = new List<Toggle>();
    public List<GameObject> panels = new List<GameObject>();

    public ToggleGroup SettingsToggleGroup;

    public GameObject FPSPanel;

    public PauseMenuScript pauseMenuScript;

    public List<GameObject> hideInWebGLBuild = new List<GameObject>();
    public List<GameObject> hideInWindowsBuild = new List<GameObject>();

    // Declare a delegate and event
    public delegate void ResetSettingsToDefault();
    public static event ResetSettingsToDefault OnResetSettingsToDefault;

    public delegate void ShowPlayerHealthBar(bool show);
    public static event ShowPlayerHealthBar OnShowPlayerHealthBar;

    public delegate void ShowEnemyHealthBar(bool show);
    public static event ShowEnemyHealthBar OnShowEnemyHealthBar;

    public delegate void EnablePostProcessingBloomEnable(bool show);
    public static event EnablePostProcessingBloomEnable OnEnableBloom;

    public delegate void UpdateInfos();
    public static event UpdateInfos updateInfos;

    private void Start()
    {
        // hide some of the settings based on build type
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // web build
            foreach(GameObject go in hideInWebGLBuild)
            {
                go.SetActive(false);
            }
        }
        else
        {
            // windows build
            foreach(GameObject go in hideInWindowsBuild)
            {
                go.SetActive(false);
            }
        }


        SettingsTabChanged(panels[0]);
    }

    public void SettingsTabChanged(GameObject go)
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(panel == go);
        }
    }

    public void ValueChanged<T>(ISettings.Type type, T value) where T : struct
    {
        switch (value)
        {
            case float floatValue:
                PlayerPrefs.SetFloat(type.ToString(), floatValue);
                PlayerPrefs.Save();
                break;
            case int intValue:
                PlayerPrefs.SetInt(type.ToString(), intValue);
                PlayerPrefs.Save();
                break;
            case bool boolValue:
                // PlayerPrefs doesn't support bool values so we convert it to integer and have to remember to convert it back when needed
                PlayerPrefs.SetInt(type.ToString(), (boolValue ? 1 : 0));
                PlayerPrefs.Save();
                break;
            default:
                Debug.Log("Unsupported type: " + typeof(T));
                break;
        }

        Debug.Log("" + type.ToString() + ": " + value + " saved to PlayerPrefs");
    }

    public void ResetToDefaultButtonClicked()
    {
        Debug.Log("ResetSettings");
        OnResetSettingsToDefault?.Invoke();
    }

    public void ShowPlayerHealthBarInGameScene(bool show)
    {
        OnShowPlayerHealthBar?.Invoke(show);
    }

    public void ShowEnemyHealthBarInGameScene(bool show)
    {
        OnShowEnemyHealthBar?.Invoke(show);
    }

    public void EnableBloom(bool enable)
    {
        OnEnableBloom?.Invoke(enable);
    }

    public void ShowFPSPanel(bool show)
    {
        FPSPanel.SetActive(show);
    }

    public void MuteMusicOnPause(bool mute)
    {
        if(pauseMenuScript != null)
        {
            AudioManager.Instance.PauseMusic(mute);
        }
    }

    public void UpdateAllInfos()
    {
        updateInfos?.Invoke();
    }
}
