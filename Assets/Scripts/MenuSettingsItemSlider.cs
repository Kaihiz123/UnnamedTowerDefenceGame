using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsItemSlider : MonoBehaviour
{

    ISettings settings;

    
    public ISettings.Type type;

    public TMPro.TextMeshProUGUI SliderValueText;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    public int minValue = 0;
    public int maxValue = 100;
    public string unit = "";

    Slider slider;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        slider = GetComponentInChildren<Slider>();

        slider.minValue = minValue;
        slider.maxValue = maxValue;

        itemText.text = itemName;

        //TODO: change to current value

        SliderValueChanged();
    }

    public void SliderValueChanged()
    {
        int roundedValue = Mathf.RoundToInt(slider.value);
        SliderValueText.text = roundedValue + unit;
        settings.ValueChanged(type, roundedValue);
    }
}
