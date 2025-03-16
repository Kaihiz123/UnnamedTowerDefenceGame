using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SelectionWindowItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public SelectionWindow selectionWindow;
    public int price;
    public TowerInfo.TowerType towerType;
    public int upgradeIndex; // 0,1,2 are upgrades, -1 is sell

    Image upgradeButtonImage;
    float hoverOverAlpha = 1f; // when mouse is above button
    float defaultAlpha = 0.7f; // when mouse is not above button

    private void Start()
    {
        if(upgradeButtonImage == null)
        {
            upgradeButtonImage = GetComponent<Image>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        selectionWindow.MouseButtonDown(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        selectionWindow.MouseButtonUp(gameObject, upgradeIndex, price);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // indicate that cursor is above the button
        Color color = upgradeButtonImage.color;
        color.a = hoverOverAlpha;
        upgradeButtonImage.color = color;

        selectionWindow.HoverOverButtonEnter(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // indicate that cursor is no longer above the button
        Color color = upgradeButtonImage.color;
        color.a = defaultAlpha;
        upgradeButtonImage.color = color;

        selectionWindow.HoverOverButtonExit(gameObject);
    }
}
