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
    public bool isInteger;
    public string unit = "";

    Slider slider;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        slider = GetComponentInChildren<Slider>();

        itemText.text = itemName;

        if (isInteger)
        {
            slider.wholeNumbers = true;
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.value = PlayerPrefs.GetInt(type.ToString(), 0);
        }
        else
        {
            slider.wholeNumbers = false;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = PlayerPrefs.GetFloat(type.ToString(), 0f);
        }
        
        SliderValueChanged();
    }

    public void SliderValueChanged()
    {
        if(isInteger)
        {
            SliderValueText.text = Mathf.RoundToInt(slider.value) + unit;
            settings.ValueChanged(type, Mathf.RoundToInt(slider.value));
        }
        else
        {
            SliderValueText.text = Mathf.RoundToInt((slider.value * maxValue) - minValue) + unit;
            AudioManager.Instance.SetVolume(type, slider.value);
            settings.ValueChanged(type, slider.value);
        }
    }
}
