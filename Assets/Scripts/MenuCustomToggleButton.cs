using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuCustomToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color selectedColor;
    public Color highlightSelectedColor;
    public Color defaultColor;
    public Color highlightDefaultColor;
    Toggle toggle;

    bool isSelected = false;
    Image targetImage;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        targetImage = toggle.targetGraphic as Image;
        isSelected = toggle.isOn;
        ChangeColor();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void UpdateValue()
    {
        Debug.Log("UpdateValue");

        isSelected = toggle.isOn;
        ChangeColor();
        overrideOnce = true;
    }

    bool overrideOnce = false;

    private void OnToggleValueChanged(bool isOn)
    {
        if(overrideOnce)
        {
            overrideOnce = false;
        }
        else
        {
            isSelected = isOn;
            ChangeColor();
        }        
    }

    private void ChangeColor()
    {
        targetImage.color = isSelected ? selectedColor : defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetImage.color = isSelected ? highlightSelectedColor : highlightDefaultColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetImage.color = isSelected ? selectedColor : defaultColor;
    }
}
