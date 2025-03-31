using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public StoreHandler storeHandler;
    public Image towerImageUI;
    public TMPro.TextMeshProUGUI towerText;
    public TowerInfo.TowerType towerType;

    bool playerCanAffordThisItem = true;
    bool mouseIsHoveringOverThisItem = false;

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

        mouseIsHoveringOverThisItem = true;
        ChangeColor();

        storeHandler.CursorEnterStoreItem(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // The cursor left the storeItem area

        mouseIsHoveringOverThisItem = false;
        ChangeColor();

        storeHandler.CursorExitStoreItem(gameObject);
    }

    public void PlayerCanAfford()
    {
        playerCanAffordThisItem = true;
        ChangeColor();
    }

    public void PlayerCannotAfford()
    {
        playerCanAffordThisItem = false;
        ChangeColor();
    }

    private void ChangeColor()
    {
        float hoverOverAlpha = 0.5f;
        float defaultAlpha = 1f;

        if (playerCanAffordThisItem)
        {
            if (mouseIsHoveringOverThisItem)
            {
                towerImageUI.color = new Color(1f, 1f, 1f, hoverOverAlpha);
                towerText.color = new Color(1f, 1f, 0.5f, 1f);
            }
            else
            {
                towerImageUI.color = new Color(1f, 1f, 1f, defaultAlpha);
                towerText.color = new Color(1f, 1f, 0.5f, 1f);
            }
        }
        else
        {
            towerImageUI.color = new Color(0.33f, 0.33f, 0.33f, defaultAlpha);
            towerText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
    }
}
