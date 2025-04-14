using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class SelectionWindowItem : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public int upgradeIndex; // 0,1,2 are upgrades, -1 is sell

    public TMPro.TextMeshProUGUI buttonText;
    public Image upgradeButtonImage;

    UpgradeLayoutScript upgradeLayoutScript;
    
    private void Start()
    {
        if(upgradeLayoutScript == null)
        {
            upgradeLayoutScript = GetComponentInParent<UpgradeLayoutScript>();
        }

        if(upgradeIndex == 0)
        {
            playerOwnsThisItem = true;
            ChangeColor();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        upgradeLayoutScript.MouseButtonDown(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        upgradeLayoutScript.MouseButtonUp(gameObject, upgradeIndex, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // indicate that cursor is above the button
        mouseIsHoveringOverThisItem = true;
        ChangeColor();

        upgradeLayoutScript.HoverOverButtonEnter(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // indicate that cursor is no longer above the button
        mouseIsHoveringOverThisItem = false;
        ChangeColor();

        upgradeLayoutScript.HoverOverButtonExit(gameObject);
    }

    bool playerCanAffordThisItem;
    bool mouseIsHoveringOverThisItem;
    bool playerOwnsThisItem;

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

    public void PlayerOwns(bool owns)
    {
        playerOwnsThisItem = owns;
        ChangeColor();
    }

    private void ChangeColor()
    {
        float hoverOverAlpha = 0.5f;
        float defaultAlpha = 1f;

        if(upgradeIndex == -1)
        {
            // this is sell button
            if (mouseIsHoveringOverThisItem)
            {
                upgradeButtonImage.color = new Color(1f, 1f, 1f, hoverOverAlpha);
                buttonText.color = new Color(1f, 1f, 0.5f, 1f);
            }
            else
            {
                upgradeButtonImage.color = new Color(1f, 1f, 1f, defaultAlpha);
                buttonText.color = new Color(1f, 1f, 0.5f, 1f);
            }
        }
        else if (playerOwnsThisItem)
        {
            // player owns this upgrade

            upgradeButtonImage.color = new Color(1f, 1f, 1f, hoverOverAlpha);
            buttonText.color = new Color(1f, 1f, 0.5f, 1f);
        }
        else if (playerCanAffordThisItem)
        {
            if (mouseIsHoveringOverThisItem)
            {
                upgradeButtonImage.color = new Color(1f, 1f, 1f, hoverOverAlpha);
                buttonText.color = new Color(1f, 1f, 0.5f, 1f);
            }
            else
            {
                upgradeButtonImage.color = new Color(1f, 1f, 1f, defaultAlpha);
                buttonText.color = new Color(1f, 1f, 0.5f, 1f);
            }
        }
        else
        {
            // player doesnt own this and cannot afford it
            upgradeButtonImage.color = new Color(0.33f, 0.33f, 0.33f, defaultAlpha);
            buttonText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
    }

}
