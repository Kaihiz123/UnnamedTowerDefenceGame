using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject PausePanel;

    private void Start()
    {
        // close the pause menu if it's showing at the beginning
        if (PausePanel.activeInHierarchy)
        {
            PausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            Pause();
        }
    }

    private void Pause()
    {
        PausePanel.SetActive(!PausePanel.activeInHierarchy);

        if (PausePanel.activeInHierarchy)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ResumeGameButtonPressed()
    {
        Pause();
    }

    public void SettingsButtonPressed()
    {
        Debug.Log("SettingsButton");
    }

    public void ExitToMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ExitToWindowsPressed()
    {
        Debug.Log("ExitToWindows");
    }
}
