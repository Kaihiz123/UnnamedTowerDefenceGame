using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsItemPicture : MonoBehaviour
{

    public TMPro.TextMeshProUGUI itemText;
    public string itemName;

    public TMPro.TextMeshProUGUI descriptionText;
    public string description;

    public Image image;
    public Sprite sprite;
    

    private void Awake()
    {
        itemText.text = itemName;
        descriptionText.text = description;

        image.sprite = sprite;
    }

}
