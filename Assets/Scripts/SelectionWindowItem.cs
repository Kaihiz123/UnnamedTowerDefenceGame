using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SelectionWindowItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public int upgradeIndex; // 0,1,2 are upgrades, -1 is sell

    UpgradeLayoutScript upgradeLayoutScript;
    Image upgradeButtonImage;
    float hoverOverAlpha = 1f; // when mouse is above button
    float defaultAlpha = 0.7f; // when mouse is not above button

    private void Start()
    {
        if(upgradeButtonImage == null)
        {
            upgradeButtonImage = GetComponent<Image>();
        }
        if(upgradeLayoutScript == null)
        {
            upgradeLayoutScript = GetComponentInParent<UpgradeLayoutScript>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        upgradeLayoutScript.MouseButtonDown(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        upgradeLayoutScript.MouseButtonUp(gameObject, upgradeIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // indicate that cursor is above the button
        Color color = upgradeButtonImage.color;
        color.a = hoverOverAlpha;
        upgradeButtonImage.color = color;

        upgradeLayoutScript.HoverOverButtonEnter(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // indicate that cursor is no longer above the button
        Color color = upgradeButtonImage.color;
        color.a = defaultAlpha;
        upgradeButtonImage.color = color;

        upgradeLayoutScript.HoverOverButtonExit(gameObject);
    }
}
