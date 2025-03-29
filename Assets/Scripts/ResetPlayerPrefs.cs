using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    public bool Reset = false;

    private void OnValidate()
    {
        if (Reset)
        {
            Reset = false;

            PlayerPrefs.DeleteAll();
            Debug.Log("Deleted all player prefs");
        }
    }
}
