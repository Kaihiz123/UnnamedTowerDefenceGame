using UnityEngine;

public class UIFPSCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textField;

    public float interval = 0.5f;

    float timeElapsed = 0f;
    int frameCount = 0;

    private void Update()
    {
        frameCount++;
        timeElapsed += Time.unscaledDeltaTime;
        
        if(timeElapsed > interval)
        {
            textField.text = "FPS " + Mathf.RoundToInt((float) frameCount / timeElapsed);

            frameCount = 0;
            timeElapsed = 0f;
        }
        
    }
}
