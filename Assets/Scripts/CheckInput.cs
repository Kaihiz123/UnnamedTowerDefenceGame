using UnityEngine;

public class CheckInput : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;

    private void OnEnable()
    {
        inputField.onValidateInput += ValidateChar;
    }

    private void OnDisable()
    {
        inputField.onValidateInput -= ValidateChar;
    }

    private char ValidateChar(string text, int charIndex, char addedChar)
    {
        if (char.IsLetterOrDigit(addedChar))
        {
            return addedChar;
        }
        return '\0'; // Returning null character rejects the input
    }
}
