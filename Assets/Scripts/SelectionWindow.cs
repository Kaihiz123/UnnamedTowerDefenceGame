using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using static TowerInfo;

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
        // change the color of the upgrade buttons based on the level player has upgraded the selected tower.
        // blue button color indicates ownership and yellow indicates that it has not been purchased
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
            currentTowerInfo.gameObject.GetComponent<TowerUpgrading>().RunWhenTowerUpgrades();
            UpdateUpgradeStatus();
        }
    }

    private void SellButtonPressed(int moneyBack)
    {
        // give money back from selling a tower
        bank.IncreasePlayerMoney(moneyBack);

        tpg.SelectedTowerWasSold(currentTowerInfo);
    }

    public void MouseButtonDown(GameObject go)
    {

    }

    public void MouseButtonUp(GameObject go, int upgradeIndex, int price, TowerTypeUpgradeDataSO towerUpgrades)
    {
        // confirm that mouse button was pressed down and up on the same button
        if(hoverOverButton == go)
        {
            // -1 -> we are selling the tower
            // >0 -> we are upgrading (when tower is bought it is upgraded to first level)
            if (upgradeIndex == -1)
            {
                // give player only half the money back
                int moneyBack = 0;
                for(int i = 0; i <= currentTowerInfo.upgradeIndex; i++)
                {
                    moneyBack += towerUpgrades.towerType[(int)currentTowerInfo.towerType].upgradeLevels[i].upgradeCost;
                }
                moneyBack /= 2;

                SellButtonPressed(moneyBack);
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
