using System.Collections.Generic;
using UnityEngine;

public class MenuSettingsItemDropdown : MonoBehaviour
{
    ISettings settings;
    TMPro.TMP_Dropdown dropdown;
    public ISettings.Type type;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    public List<string> options = new List<string>();

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        dropdown = GetComponentInChildren<TMPro.TMP_Dropdown>();

        dropdown.ClearOptions();

        itemText.text = itemName;

        switch (type)
        {
            case ISettings.Type.RESOLUTION:
                Resolution[] availableResolutions = Screen.resolutions;
                Resolution currentResolution = Screen.currentResolution;
                int currentResolutionIndex = -1;
                for(int i = 0; i < availableResolutions.Length; i++)
                {
                    options.Add(availableResolutions[i].width + " x " + availableResolutions[i].height);

                    if(availableResolutions[i].Equals(currentResolution))
                    {
                        currentResolutionIndex = i;
                    }
                }
                dropdown.AddOptions(options);
                dropdown.value = currentResolutionIndex;
                dropdown.RefreshShownValue();

                dropdown.onValueChanged.AddListener(ResolutionChanged);

                break;
            default:
                dropdown.AddOptions(options);
                break;
        }
        
    }

    public void ResolutionChanged(int index)
    {
        Resolution newResolution = Screen.resolutions[index];
        Screen.SetResolution(newResolution.width, newResolution.height, FullScreenMode.MaximizedWindow);

        Debug.Log("resolution changed");
    }

    public void DropdownValueChanged(int index)
    {
        settings.ValueChanged(type, dropdown.value);
    }

    
}
