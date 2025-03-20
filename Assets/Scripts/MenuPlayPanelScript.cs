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

    public void ValueChanged(ISettings.Type type, int value)
    {
        // if value == bool -> 1 == true, 0 == false
        // if value == int -> value == index or valueNumber

        PlayerPrefs.SetInt(type.ToString(), value);
        PlayerPrefs.Save();
    }
}
