using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour, ISettings
{
    public List<Button> tabButtons = new List<Button>();
    public List<GameObject> panels = new List<GameObject>();

    int selectedIndex = 0;

    private void OnEnable()
    {
        tabButtons[selectedIndex].Select();
        SettingsTabChanged(panels[selectedIndex]);
    }

    public void SettingsTabChanged(GameObject go)
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(panel == go);
        }
        selectedIndex = panels.IndexOf(go);
    }

    public void ValueChanged(ISettings.Type type, int value)
    {
        // if value == bool -> 1 == true, 0 == false
        // if value == int -> value == index or valueNumber

        switch (type)
        {
            case ISettings.Type.MUSICVOLUME:
                Debug.Log("value=" + value);
                break;
            default:
                Debug.Log("Type mismatch: " + type.ToString());
                break;
        }
    }
}
