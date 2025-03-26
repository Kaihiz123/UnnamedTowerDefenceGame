using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsItemSlider : MonoBehaviour
{

    ISettings settings;

    public ISettings.Type type;

    public TMPro.TextMeshProUGUI SliderValueText;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    public float minValue;
    public float maxValue;
    public float defaultSliderValue;
    public bool savedAsInteger;
    public int interval;
    public string unit = "";

    Slider slider;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        slider = GetComponentInChildren<Slider>();

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        itemText.text = itemName;

        slider.wholeNumbers = false;
        slider.minValue = 0f;
        slider.maxValue = 1f;

        if (savedAsInteger)
        {
            if (PlayerPrefs.HasKey(type.ToString()))
            {
                int intValue = PlayerPrefs.GetInt(type.ToString());
                slider.value = ((float)intValue - minValue) / (maxValue - minValue);
            }
            else
            {
                slider.value = defaultSliderValue;
            }
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat(type.ToString(), defaultSliderValue);
        }

        SliderValueChanged();
    }

    public void SliderValueChanged()
    {
        if(savedAsInteger)
        {
            if(interval == 0)
            {
                interval = 1;
            }

            SliderValueText.text = "" + Mathf.RoundToInt((slider.value * (maxValue - minValue) + minValue) / (float)interval) * interval + unit;
            settings.ValueChanged(type, Mathf.RoundToInt((slider.value * (maxValue - minValue) + minValue) / (float)interval) * interval);
        }
        else
        {
            SliderValueText.text = Mathf.RoundToInt(slider.value * (maxValue - minValue) + minValue) + unit;
            AudioManager.Instance.SetVolume(type, slider.value);
            settings.ValueChanged(type, slider.value);
        }
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
