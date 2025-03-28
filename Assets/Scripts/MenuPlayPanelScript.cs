using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayPanelScript : MonoBehaviour, ISettings
{
    // Declare a delegate and event
    public delegate void ResetSettingsToDefault();
    public static event ResetSettingsToDefault OnResetSettingsToDefault;

    public void ResetToDefaultButtonClicked()
    {
        Debug.Log("ResetSettings");
        OnResetSettingsToDefault?.Invoke();
    }

    public void PlayButtonClicked()
    {
        Debug.Log("PlayButtonClicked");
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ValueChanged<T>(ISettings.Type type, T newValue) where T : struct
    {
        if(newValue is int intValue)
        {
            PlayerPrefs.SetInt(type.ToString(), intValue);
            PlayerPrefs.Save();
        }
    }

    public void ShowFPSPanel(bool show)
    {
        // this is used in MenuSettingsPanelScript
    }

    public void ShowPlayerHealthBarInGameScene(bool show)
    {
        // this is used in MenuSettingsPanelScript
    }

    public void ShowEnemyHealthBarInGameScene(bool show)
    {
        // this is used in MenuSettingsPanelScript
    }

    public void EnableBloom(bool enable)
    {
        // this is used in MenuSettingsPanelScript
    }
}
