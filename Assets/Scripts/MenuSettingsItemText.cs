using UnityEngine;

public class MenuSettingsItemText : MonoBehaviour
{
    
    public TMPro.TextMeshProUGUI itemText1;
    public string itemName1;

    public TMPro.TextMeshProUGUI itemText2;
    public string itemName2;

    private void Awake()
    {
        itemText1.text = itemName1;
        itemText2.text = itemName2;
    }

}
