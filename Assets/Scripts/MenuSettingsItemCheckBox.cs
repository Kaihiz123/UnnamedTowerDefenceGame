using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsItemCheckBox : MonoBehaviour
{
    ISettings settings;
    public ISettings.Type type;
    Toggle toggle;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        toggle = GetComponentInChildren<Toggle>();

        itemText.text = itemName;

        switch (type)
        {
            case ISettings.Type.FULLSCREEN:
                toggle.isOn = Screen.fullScreen;
                break;
            default:

                break;
        }

        toggle.onValueChanged.AddListener(CheckBoxValueChanged);
    }

    public void CheckBoxValueChanged(bool isOn)
    {
        switch (type)
        {
            case ISettings.Type.FULLSCREEN:
                Screen.fullScreen = isOn;
                Debug.Log("fullscreen = " + isOn);
                break;
            default:
                break;
        }

        settings.ValueChanged(type, toggle.isOn ? 1 : 0);
    }
}
