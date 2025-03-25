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
    public bool savedAsInteger;
    public int interval;
    public string unit = "";

    Slider slider;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        slider = GetComponentInChildren<Slider>();

        itemText.text = itemName;

        slider.wholeNumbers = false;
        slider.minValue = 0f;
        slider.maxValue = 1f;

        if (savedAsInteger)
        {
            slider.value = ((float)PlayerPrefs.GetInt(type.ToString(), Mathf.RoundToInt(minValue)) - minValue) / (maxValue - minValue);
        }
        else
        {
            slider.value = PlayerPrefs.GetFloat(type.ToString(), 0f);
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
}
