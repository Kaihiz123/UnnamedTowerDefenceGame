using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

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

    public void SaveButtonClicked()
    {
        Debug.Log("SaveButtonClicked");
    }

    public void PlayButtonClicked()
    {
        Debug.Log("PlayButtonClicked");

        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void ExitButtonClicked()
    {
        Debug.Log("ExitButtonClicked");
    }
}
