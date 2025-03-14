using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public StoreHandler storeHandler;
    public int index;
    public int price;
    public Image towerImageUI;
    public TMPro.TextMeshProUGUI towerText;
    public TowerInfo.TowerType towerType;

    float hoverOverAlpha = 1f;
    float defaultAlpha = 0.7f;

    public void Init()
    {
        if(storeHandler == null)
        {
            storeHandler = GetComponentInParent<StoreItemHandler>(true).storeHandler;
        }

        // the index of the storeItem in the store
        index = transform.parent.GetSiblingIndex();

        // debug (if no price is given to storeItem)
        if(price == 0)
        {
            price = 100 * index + 100;
        }

        // name of the tower that is shown on the storeItem
        towerText.text = towerType.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Left button of the mouse was pressed down
        storeHandler.MouseButtonDownOnStoreItem(gameObject, index);
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
        towerImageUI.color = new Color(0f, 0f, 1f, defaultAlpha);
    }

    public void PlayerCannotAfford()
    {
        // change color to red
        towerImageUI.color = new Color(1f, 0f, 0f, defaultAlpha);
    }
}
