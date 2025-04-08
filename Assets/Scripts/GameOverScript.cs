using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI playerStatsText;
    [SerializeField] private GameObject GameOverScreen;

    public void ShowGameOverScreen()
    {

        //TODO: create player stats text

        GameOverScreen.SetActive(true);
    }

    public void BackToMainMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
