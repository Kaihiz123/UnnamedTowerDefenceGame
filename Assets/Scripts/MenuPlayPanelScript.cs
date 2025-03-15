using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayPanelScript : MonoBehaviour
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
}
