using UnityEngine;

public class UIFPSCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textField;

    private void Update()
    {
        textField.text = "FPS " + (int)(1f / Time.unscaledDeltaTime);
    }
}
