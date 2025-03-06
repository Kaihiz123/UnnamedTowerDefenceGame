using UnityEngine;
using UnityEngine.EventSystems;

public class StoreItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public StoreHandler storeHandler;

    public void OnPointerDown(PointerEventData eventData)
    {
        storeHandler.MouseButtonDownOnStoreItem(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        storeHandler.MouseButtonUpOnStoreItem(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        storeHandler.CursorEnterStoreItem(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        storeHandler.CursorExitStoreItem(gameObject);
    }
}
