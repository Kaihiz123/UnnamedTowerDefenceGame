using UnityEngine;
using UnityEngine.UI;

public class HiscoreSubmitButton : MonoBehaviour
{
    public GameOverScript gameOverScript;
    public TMPro.TMP_InputField playerNameInputField;

    Button submitButton;

    private void Start()
    {
        submitButton = GetComponent<Button>();
        submitButton.interactable = true;
    }

    public void Disable()
    {
        submitButton.interactable = false;
    }

    public void Submit()
    {
        UGSManager.Instance.SubmitScore(playerNameInputField.text);
        submitButton.interactable = false;
    }
}
