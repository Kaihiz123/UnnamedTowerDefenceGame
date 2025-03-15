using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MenuSettingsPanelScript : MonoBehaviour, ISettings
{
    public List<Toggle> toggles = new List<Toggle>();
    public List<GameObject> panels = new List<GameObject>();

    public ToggleGroup SettingsToggleGroup;

    private void Start()
    {
        SettingsTabChanged(panels[0]);
    }

    public void SettingsTabChanged(GameObject go)
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(panel == go);
        }
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

    public void ResetToDefaultButtonClicked()
    {
        Debug.Log("ResetToDefaultButtonClicked");
    }
}
