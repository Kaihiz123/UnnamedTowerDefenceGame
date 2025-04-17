using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayPanelScript : MonoBehaviour, ISettings
{
    // Declare a delegate and event
    public delegate void ResetSettingsToDefault();
    public static event ResetSettingsToDefault OnResetSettingsToDefault;

    public GameObject notEligibleToLeaderboardsTextPanel;
    public DefaultPlayerValues defaultPlayerValues;

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

        

        bool playerEligibleToLeaderboards = PlayerPrefs.GetInt(ISettings.Type.STARTMONEY.ToString()) <= defaultPlayerValues.defaultPlayerStartMoney 
            && PlayerPrefs.GetInt(ISettings.Type.STARTHEALTH.ToString()) <= defaultPlayerValues.defaultPlayerStartHealth
            && PlayerPrefs.GetInt(ISettings.Type.MAXHEALTH.ToString()) <= defaultPlayerValues.defaultPlayerMaxHealth;
        notEligibleToLeaderboardsTextPanel.SetActive(!playerEligibleToLeaderboards);
        PlayerPrefs.SetInt("PlayerEligibleToLeaderboards", playerEligibleToLeaderboards ? 1 : 0);
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

    public void MuteMusicOnPause(bool mute)
    {
        // this is used in pause menu
    }

    public void UpdateAllInfos()
    {
        // this is used in MenuSettingsPanelScript
    }
}
