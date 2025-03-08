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
        towerText.text = "Tower" + (index + 1);
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
        // The cursor is hovering the storeItem
        storeHandler.CursorEnterStoreItem(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // The cursor left storeItem area
        storeHandler.CursorExitStoreItem(gameObject);
    }

    public void PlayerCanAfford()
    {
        // change color to blue
        towerImageUI.color = Color.blue;
    }

    public void PlayerCannotAfford()
    {
        // change color to red
        towerImageUI.color = Color.red;
    }
}
