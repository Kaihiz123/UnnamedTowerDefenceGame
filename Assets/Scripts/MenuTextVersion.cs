using UnityEngine;
using TMPro;

public class MenuTextVersion : MonoBehaviour
{
    public TMP_Text versionText; // Assign in Inspector

    void Start()
    {
        if (versionText == null)
        {
            Debug.LogError("VersionDisplay: No TMP_Text component assigned.");
            return;
        }
        
        versionText.text = "Version: " + Application.version;
    }
}
