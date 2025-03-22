using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayPanelScript : MonoBehaviour, ISettings
{

    public void ResetToDefaultButtonClicked()
    {
        Debug.Log("ResetToDefaultButtonClicked");
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
