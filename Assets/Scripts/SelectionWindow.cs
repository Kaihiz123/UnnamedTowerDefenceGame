using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class SelectionWindow : MonoBehaviour
{

    public TextMeshProUGUI towerNameText;

    TowerInfo currentTowerInfo;

    public Bank bank;
    public TowerPlacementGrid tpg;

    public GameObject TowerType1Layout;
    public GameObject TowerType2Layout;
    public GameObject TowerType3Layout;

    List<GameObject> layouts = new List<GameObject>();

    public Color ownedColor;
    public Color notOwnedColor;

    public void Init(TowerInfo towerInfo)
    {
        layouts.Clear();
        layouts.Add(TowerType1Layout);
        layouts.Add(TowerType2Layout);
        layouts.Add(TowerType3Layout);

        currentTowerInfo = towerInfo;
        towerNameText.text = towerInfo.towerType.ToString();

        layouts[(int) towerInfo.towerType].SetActive(true);

        UpdateUpgradeStatus();
    }

    public void CloseSelectionWindow()
    {
        if(currentTowerInfo != null)
        {
            layouts[(int)currentTowerInfo.towerType].SetActive(false);
        }
    }

    private void UpdateUpgradeStatus()
    {
        // k‰ytet‰‰n towerInfo.upgradeIndexia tutkimaan mink‰ v‰risi‰ upgradet ovat layoutissa
        // sininen on omistama, vihre‰ on rahat riitt‰‰, punainen on ettei rahat riit‰
        SelectionWindowItem[] items = GetComponentsInChildren<SelectionWindowItem>();
        foreach(SelectionWindowItem item in items)
        {
            if (item.gameObject.name.Contains("Upgrade"))
            {
                if(currentTowerInfo.upgradeIndex >= item.upgradeIndex)
                {
                    item.gameObject.GetComponent<Image>().color = ownedColor;
                }
                else
                {
                    item.gameObject.GetComponent<Image>().color = notOwnedColor;
                }
            }
        }
    }

    private void UpgradeButtonPressed(int upgradeNumber, int price)
    {
        // if player has enough money increment towerInfos upgradeIndex by 1 and updateUpgradeStatus
        if (bank.BuyUpgrade(price))
        {
            currentTowerInfo.upgradeIndex = upgradeNumber;
            UpdateUpgradeStatus();
        }
    }

    private void SellButtonPressed()
    {
        //TODO: give money back from selling a tower

        tpg.SelectedTowerWasSold(currentTowerInfo);
    }

    public void MouseButtonUp(GameObject go, int upgradeIndex, int price)
    {
        // confirm that mouse button was pressed down and up on the same button
        if(hoverOverButton == go)
        {
            if (upgradeIndex == -1)
            {
                SellButtonPressed();
            }
            else
            {
                // Check that this upgrade haven't been bought already. Also prevent player skipping upgrades.
                if(currentTowerInfo.upgradeIndex + 1 == upgradeIndex)
                {
                    UpgradeButtonPressed(upgradeIndex, price);
                }
            }
        }
    }

    GameObject hoverOverButton = null;
    public void HoverOverButtonEnter(GameObject go)
    {
        hoverOverButton = go;
    }

    public void HoverOverButtonExit(GameObject go)
    {
        hoverOverButton = null;
    }

}
