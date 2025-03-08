using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SelectionWindowItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public SelectionWindow selectionWindow;
    public int price;
    public TowerInfo.TowerType towerType;
    public int upgradeIndex; // 0,1,2 are upgrades, -1 is sell

    public void OnPointerDown(PointerEventData eventData)
    {
        //selectionWindow.MouseButtonDown(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        selectionWindow.MouseButtonUp(gameObject, upgradeIndex, price);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //TODO: indicate that cursor is above a button

        selectionWindow.HoverOverButtonEnter(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //TODO: indicate that cursor is no longer above button

        selectionWindow.HoverOverButtonExit(gameObject);
    }
}
