using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour, IConfirmation
{

    public ConfirmationWindow confirmationWindow;

    public List<GameObject> panels = new List<GameObject>();

    public Toggle ExitButton;

    // indeces of panels
    // 0 == Play
    // 1 == Hiscore
    // 2 == Settings
    // 3 == Credits

    GameObject currentPanel = null;

    public AudioClip mainMenuMusicAudioClip;

    private void Start()
    {
        if(mainMenuMusicAudioClip != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuMusicAudioClip);
        }
    }

    // this is called from unity button in the left panel in the main menu
    public void ShowPanel(GameObject go)
    {
        currentPanel = (currentPanel == go) ? null : go;
        foreach(GameObject panel in panels)
        {
            panel.SetActive(panel == currentPanel);
        }
    }

    public void ExitButtonClicked()
    {
        confirmationWindow.Init(this, "ExitButton", "You are about to exit to Windows, are You sure?");
        confirmationWindow.gameObject.SetActive(true);
        ExitButton.isOn = false;
        ExitButton.gameObject.GetComponent<MenuCustomToggleButton>().UpdateValue();
    }

    public void ConfirmationSucceeded(string whoAddressed)
    {
        if (whoAddressed.Equals("ExitButton"))
        {
#if UNITY_EDITOR
            Debug.Log("ExitButtonClicked, real exiting is blocked in editor");
            confirmationWindow.gameObject.SetActive(false);
# else
            Application.Quit();
#endif
        }
    }

    public void ConfirmationCanceled(string whoAddressed)
    {
        if (whoAddressed.Equals("ExitButton"))
        {
            Debug.Log("Exiting cancelled");
            confirmationWindow.gameObject.SetActive(false);
        }
    }
}
