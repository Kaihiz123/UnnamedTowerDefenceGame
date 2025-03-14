using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour, IConfirmation
{

    public ConfirmationWindow confirmationWindow;

    public List<GameObject> panels = new List<GameObject>();

    // indeces of panels
    // 0 == Play
    // 1 == Hiscore
    // 2 == Settings
    // 3 == Credits

    int selectedPanel = -1;

    // this is called from unity button in the left panel in the main menu
    public void ShowPanel(int index)
    {
        if(index == selectedPanel)
        {
            // Same button was clicked again
            panels[index].SetActive(false);
            selectedPanel = -1;
        }
        else
        {
            selectedPanel = index;
            // activate wanted panel and deactivate others
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].SetActive(i == index);
            }
        }
    }

    public void ResetToDefaultButtonClicked()
    {
        Debug.Log("ResetToDefaultButtonClicked");
    }

    public void PlayButtonClicked()
    {
        Debug.Log("PlayButtonClicked");

        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ExitButtonClicked()
    {
        confirmationWindow.Init(this, "ExitButton", "You are about to exit to Windows, are You sure?");
        confirmationWindow.gameObject.SetActive(true);
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
