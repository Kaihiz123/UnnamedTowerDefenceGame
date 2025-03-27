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
}
