using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour, IConfirmation
{
    public GameObject PauseWindow;

    public ConfirmationWindow confirmationWindow;

    public GameObject settingsPanel;

    private GameTimer gameTimer;

    private void Start()
    {
        gameTimer = FindFirstObjectByType<GameTimer>();
        // close the pause menu if it's showing at the beginning
        if (PauseWindow.activeInHierarchy)
        {
            PauseWindow.SetActive(false);
        }
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.P))
        {
            Pause();
        }
#else
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.P))
        {
            Pause();
        }
#endif

    }

    private void Pause()
    {
        PauseWindow.SetActive(!PauseWindow.activeInHierarchy);

        if(PlayerPrefs.GetInt(ISettings.Type.MUTEMUSICONPAUSE.ToString()) == 1)
        {
            AudioManager.Instance.PauseMusic(PauseWindow.activeInHierarchy);
        }
        else
        {
            AudioManager.Instance.PauseMusic(false);
        }

        if (PauseWindow.activeInHierarchy)
        {
            Time.timeScale = 0f;
            // Pause the timer when game is paused
            if (gameTimer != null)
            {
                gameTimer.PauseTimer(true);
            }
        }
        else
        {
            Time.timeScale = 1f;
            // Unpause the timer when game is resumed
            if (gameTimer != null)
            {
                gameTimer.PauseTimer(false);
            }
        }
    }

    public void ResumeGameButtonPressed()
    {
        Pause();
    }

    public void SettingsButtonPressed()
    {
        settingsPanel.SetActive(true);
    }

    public void BackButtonPressed()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitToMainMenuPressed()
    {
        confirmationWindow.Init(this, "ExitToMainMenu", "You are about to exit to main menu, are You sure?");
        confirmationWindow.gameObject.SetActive(true);
    }

    public void ExitToWindowsPressed()
    {
        confirmationWindow.Init(this, "ExitToWindows" , "You are about to exit to Windows, are You sure?");
        confirmationWindow.gameObject.SetActive(true);
    }

    public void ConfirmationSucceeded(string whoAddressed)
    {
        if (whoAddressed.Equals("ExitToWindows"))
        {
#if UNITY_EDITOR
            Debug.Log("ExitButtonClicked, real exiting is blocked in editor");
            confirmationWindow.gameObject.SetActive(false);
# else
            Application.Quit();
#endif
        }
        else if (whoAddressed.Equals("ExitToMainMenu"))
        {
            Time.timeScale = 1f;
            AudioManager.Instance.PauseMusic(false);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    public void ConfirmationCanceled(string whoAddressed)
    {
        // whoever addressed we cancel
        Debug.Log("Exiting cancelled");
        confirmationWindow.gameObject.SetActive(false);
    }
}
