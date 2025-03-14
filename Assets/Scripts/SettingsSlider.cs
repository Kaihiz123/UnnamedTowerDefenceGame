using UnityEngine;
using UnityEngine.UI;

public class SettingsSlider : MonoBehaviour
{

    ISettings settings;

    
    public ISettings.Type type;

    public TMPro.TextMeshProUGUI SliderValueText;

    Slider slider;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        slider = GetComponent<Slider>();

        //TODO: change to current value

        
    }

    public void SliderValueChanged()
    {
        int roundedValue = Mathf.RoundToInt(slider.value);
        SliderValueText.text = roundedValue + "%";
        settings.ValueChanged(type, roundedValue);
    }
}
