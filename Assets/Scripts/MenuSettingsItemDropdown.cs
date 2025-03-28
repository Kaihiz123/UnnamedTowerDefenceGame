using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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

                dropdown.onValueChanged.RemoveAllListeners();
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

                dropdown.onValueChanged.RemoveAllListeners();
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
            case ISettings.Type.ANTIALIAS:
                dropdown.onValueChanged.RemoveAllListeners();
                dropdown.AddOptions(options);
                // Valid values are 0(no MSAA), 2, 4, and 8
                int[] intArray = new int[] { 0, 2, 4, 8 };
                dropdown.value = intArray[PlayerPrefs.GetInt(type.ToString())];
                dropdown.RefreshShownValue();
                dropdown.onValueChanged.AddListener(DropdownValueChanged);
                break;
            default:
                dropdown.onValueChanged.RemoveAllListeners();
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
        switch (type)
        {
            case ISettings.Type.REFRESHRATE:
                // 0 -> 30
                // 1 -> 60
                // 2 -> unlimited
                Application.targetFrameRate = index == 0 ? 30 : index == 1 ? 60 : -1;
                break;
            case ISettings.Type.WINDOWMODE:
                FullScreenMode[] fullScreenModes = new FullScreenMode[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.MaximizedWindow };
                Screen.fullScreenMode = fullScreenModes[index];
                break;
            case ISettings.Type.ANTIALIAS:
                // Valid values are 0(no MSAA), 2, 4, and 8
                int[] intArray = new int[] { 0, 2, 4, 8 };
                QualitySettings.antiAliasing = intArray[index];
                break;
            default:
                break;
        }

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
