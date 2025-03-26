using System.Collections.Generic;
using UnityEngine;

public class MenuSettingsItemDropdown : MonoBehaviour
{
    ISettings settings;
    TMPro.TMP_Dropdown dropdown;
    public ISettings.Type type;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;
    public int defaultIndex;
    public List<string> options = new List<string>();

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        dropdown = GetComponentInChildren<TMPro.TMP_Dropdown>();

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        dropdown.ClearOptions();

        itemText.text = itemName;

        switch (type)
        {
            case ISettings.Type.UIRESOLUTION:

                options.Clear();

                Resolution[] availableResolutions = Screen.resolutions;
                Resolution currentResolution = Screen.currentResolution;
                int currentResolutionIndex = -1;
                for (int i = 0; i < availableResolutions.Length; i++)
                {
                    options.Add(availableResolutions[i].width + " x " + availableResolutions[i].height);

                    if (availableResolutions[i].Equals(currentResolution))
                    {
                        currentResolutionIndex = i;
                    }
                }
                dropdown.AddOptions(options);
                dropdown.value = currentResolutionIndex;
                dropdown.RefreshShownValue();
                dropdown.onValueChanged.AddListener(ResolutionChanged);

                break;

            case ISettings.Type.WINDOWMODE:

                options.Clear();

                FullScreenMode[] fullScreenModes = new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.MaximizedWindow };
                FullScreenMode currentFullScreenMode = Screen.fullScreenMode;
                int currentFullScreenModeIndex = (int)currentFullScreenMode;
                for (int i = 0; i < fullScreenModes.Length; i++)
                {
                    options.Add(fullScreenModes[i].ToString());
                }
                dropdown.AddOptions(options);
                dropdown.value = currentFullScreenModeIndex;
                dropdown.RefreshShownValue();
                dropdown.onValueChanged.AddListener(DropdownValueChanged);

                break;
            default:
                dropdown.AddOptions(options);
                dropdown.value = PlayerPrefs.GetInt(type.ToString(), defaultIndex);
                dropdown.RefreshShownValue();
                dropdown.onValueChanged.AddListener(DropdownValueChanged);
                break;
        }
    }

    public void ResolutionChanged(int index)
    {
        Resolution newResolution = Screen.resolutions[index];
        FullScreenMode[] fullScreenModes = new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.MaximizedWindow };
        Screen.SetResolution(newResolution.width, newResolution.height, fullScreenModes[PlayerPrefs.GetInt(ISettings.Type.WINDOWMODE.ToString(), 0)]);

        Debug.Log("resolution changed");
    }

    public void DropdownValueChanged(int index)
    {
        settings.ValueChanged(type, dropdown.value);
    }

    private void ResetSettings()
    {
        PlayerPrefs.DeleteKey(type.ToString());
        UpdateInfo();
    }

    private void OnEnable()
    {
        MenuPlayPanelScript.OnResetSettingsToDefault += ResetSettings;
        MenuSettingsPanelScript.OnResetSettingsToDefault += ResetSettings;
    }

    private void OnDisable()
    {
        MenuPlayPanelScript.OnResetSettingsToDefault -= ResetSettings;
        MenuSettingsPanelScript.OnResetSettingsToDefault -= ResetSettings;
    }
}
