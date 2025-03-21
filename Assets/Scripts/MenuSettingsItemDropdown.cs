using System.Collections.Generic;
using UnityEngine;

public class MenuSettingsItemDropdown : MonoBehaviour
{
    ISettings settings;
    TMPro.TMP_Dropdown dropdown;
    public ISettings.Type type;

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    public List<string> options = new List<string>();

    private void Awake()
    {
        settings = GetComponentInParent<ISettings>();
        dropdown = GetComponentInChildren<TMPro.TMP_Dropdown>();

        dropdown.ClearOptions(); // Remove existing options
        dropdown.AddOptions(options);

        itemText.text = itemName;

        //TODO: change to current value
    }

    public void DropdownValueChanged()
    {
        settings.ValueChanged(type, dropdown.value);
    }

    
}
