using UnityEngine;

public class ButtonDebugToggle : MonoBehaviour
{
    public void toggleDebug()
    {
        SettingsManager.Instance.DebugON = !SettingsManager.Instance.DebugON;
    }
}
