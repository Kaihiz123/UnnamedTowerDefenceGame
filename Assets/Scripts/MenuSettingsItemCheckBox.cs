using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsItemCheckBox : MonoBehaviour
{
    ISettings settings;
    public ISettings.Type type;
    Toggle toggle;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    public bool isOnByDefault;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        toggle = GetComponentInChildren<Toggle>();

        toggle.onValueChanged.AddListener(CheckBoxValueChanged);

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        itemText.text = itemName;

        switch (type)
        {
            case ISettings.Type.FULLSCREEN:
                toggle.isOn = Screen.fullScreen;
                break;
            case ISettings.Type.VERTICALSYNC:
                toggle.isOn = QualitySettings.vSyncCount == 1;
                break;
            default:
                toggle.isOn = PlayerPrefs.GetInt(type.ToString(), isOnByDefault ? 1 : 0) == 1;
                break;
        }
    }

    public void CheckBoxValueChanged(bool isOn)
    {
        switch (type)
        {
            case ISettings.Type.FULLSCREEN:
                Screen.fullScreen = toggle.isOn;
                break;
            case ISettings.Type.SHOWFPS:
                settings.ShowFPSPanel(toggle.isOn);
                break;
            case ISettings.Type.VERTICALSYNC:
                QualitySettings.vSyncCount = toggle.isOn ? 1 : 0;
                break;
            case ISettings.Type.SHOWPLAYERHEALTHBAR:
                settings.ShowPlayerHealthBarInGameScene(toggle.isOn);
                break;
            case ISettings.Type.SHOWENEMYHEALTHBAR:
                settings.ShowEnemyHealthBarInGameScene(toggle.isOn);
                break;
            case ISettings.Type.BLOOM:
                settings.EnableBloom(toggle.isOn);
                break;
            default:
                break;
        }

        settings.ValueChanged(type, toggle.isOn ? 1 : 0);
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
