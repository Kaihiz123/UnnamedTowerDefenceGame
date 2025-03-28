using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using static TowerInfo;
using System;

public class SelectionWindow : MonoBehaviour
{
    public TextMeshProUGUI towerNameText;

    TowerInfo currentTowerInfo;

    public Bank bank;
    public TowerPlacementGrid tpg;

    [Header("Tower Type Upgrades")]
    public TowerTypeUpgradeDataSO towerUpgrades;

    public List<UpgradeLayoutScript> upgradeLayoutScripts = new List<UpgradeLayoutScript>();

    public Color ownedColor;
    public Color notOwnedColor;

    public void Init(TowerInfo towerInfo)
    {
        currentTowerInfo = towerInfo;

        towerNameText.text = towerInfo.towerType.ToString();

        UpdateUpgradeStatus();
        upgradeLayoutScripts[(int)towerInfo.towerType].Init(towerInfo, this);
        upgradeLayoutScripts[(int)towerInfo.towerType].gameObject.SetActive(true);
    }

    public void CloseSelectionWindow()
    {
        if(currentTowerInfo != null)
        {
            upgradeLayoutScripts[(int)currentTowerInfo.towerType].gameObject.SetActive(false);
        }
    }

    private void UpdateUpgradeStatus()
    {
        // change the color of the upgrade buttons based on the level player has upgraded the selected tower.
        // blue button color indicates ownership and yellow indicates that it has not been purchased
        SelectionWindowItem[] items = GetComponentsInChildren<SelectionWindowItem>(true);
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

    public void UpgradeButtonPressed(int upgradeNumber, int price)
    {
        // if player has enough money increment towerInfos upgradeIndex by 1 and updateUpgradeStatus
        if (bank.BuyUpgrade(price))
        {
            currentTowerInfo.upgradeIndex = upgradeNumber;
            currentTowerInfo.gameObject.GetComponent<TowerUpgrading>().RunWhenTowerUpgrades();
            UpdateUpgradeStatus();
        }
    }

    public void SellButtonPressed(int moneyBack)
    {
        // give money back from selling a tower
        bank.IncreasePlayerMoney(moneyBack);

        tpg.SelectedTowerWasSold(currentTowerInfo);
    }
}
