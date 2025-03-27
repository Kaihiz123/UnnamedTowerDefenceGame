using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public StoreHandler storeHandler;
    public Image towerImageUI;
    public TMPro.TextMeshProUGUI towerText;
    public TowerInfo.TowerType towerType;

    float hoverOverAlpha = 0.5f;
    float defaultAlpha = 1f;

    public void Init(int price)
    {
        if(storeHandler == null)
        {
            storeHandler = GetComponentInParent<StoreItemHandler>(true).storeHandler;
        }

        // cost of the tower that is shown on the storeItem
        towerText.text = "" + price;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Left button of the mouse was pressed down
        storeHandler.MouseButtonDownOnStoreItem(gameObject, (int) towerType);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Left button of the mouse was released
        storeHandler.MouseButtonUpOnStoreItem(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // The cursor is hovering over the storeItem

        // change button color alpha
        Color color = towerImageUI.color;
        color.a = hoverOverAlpha;
        towerImageUI.color = color;

        storeHandler.CursorEnterStoreItem(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // The cursor left the storeItem area
        
        // change button color alpha
        Color color = towerImageUI.color;
        color.a = defaultAlpha;
        towerImageUI.color = color;

        storeHandler.CursorExitStoreItem(gameObject);
    }

    public void PlayerCanAfford()
    {
        // change color to blue
        towerImageUI.color = new Color(1f, 1f, 1f, defaultAlpha);
        towerText.color = new Color(1f, 1f, 0.5f, 1f);
    }

    public void PlayerCannotAfford()
    {
        // change color to red
        towerImageUI.color = new Color(0.33f, 0.33f, 0.33f, hoverOverAlpha);
        towerText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
    }
}
