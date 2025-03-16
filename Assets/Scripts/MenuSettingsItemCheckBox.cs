using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsItemCheckBox : MonoBehaviour
{
    ISettings settings;
    public ISettings.Type type;
    Toggle toggle;

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        toggle = GetComponentInChildren<Toggle>();

        //TODO: change to current value
    }

    public void CheckBoxValueChanged()
    {
        settings.ValueChanged(type, toggle.isOn ? 1 : 0) ;
    }
}
