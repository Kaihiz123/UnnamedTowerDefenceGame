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

    public void Init(TowerInfo towerInfo)
    {
        currentTowerInfo = towerInfo;

        UpdateNameText();
        
        upgradeLayoutScripts[(int)towerInfo.towerType].Init(towerInfo, this);
        upgradeLayoutScripts[(int)towerInfo.towerType].gameObject.SetActive(true);

        UpdateUpgradeStatus();
    }

    public void CloseSelectionWindow()
    {
        if(currentTowerInfo != null)
        {
            upgradeLayoutScripts[(int)currentTowerInfo.towerType].gameObject.SetActive(false);
        }
    }

    public void UpdateUpgradeStatus()
    {
        // change the color of the upgrade buttons based on the level player has upgraded the selected tower.
        // blue button color indicates ownership and yellow indicates that it has not been purchased

        if(currentTowerInfo == null)
        {
            return;
        }

        SelectionWindowItem[] items = GetComponentsInChildren<SelectionWindowItem>(true);
        foreach(SelectionWindowItem item in items)
        {
            if (item.gameObject.name.Contains("Upgrade"))
            {
                if(currentTowerInfo.upgradeIndex >= item.upgradeIndex)
                {
                    item.PlayerOwns(true);
                }
                else
                {
                    item.PlayerOwns(false);
                    if(bank.GetPlayerMoney() >= towerUpgrades.towerType[(int)currentTowerInfo.towerType].upgradeLevels[item.upgradeIndex].upgradeCost)
                    {
                        item.PlayerCanAfford();
                    }
                    else
                    {
                        item.PlayerCannotAfford();
                    }
                }
            }
        }
    }

    public void UpgradeButtonPressed(int upgradeNumber, int price, SelectionWindowItem selectionWindowItem)
    {
        // block if already upgrading/building
        if (!currentTowerInfo.gameObject.GetComponent<TowerShooting>().IsAvailableToUpgrade())
        {
            return;
        }
        
        // if player has enough money increment towerInfos upgradeIndex by 1 and updateUpgradeStatus
        if (bank.BuyUpgrade(price))
        {
            currentTowerInfo.upgradeIndex = upgradeNumber;
            UpdateNameText();
            currentTowerInfo.gameObject.GetComponent<TowerUpgrading>().RunWhenTowerUpgrades();
            currentTowerInfo.gameObject.GetComponent<TowerShooting>().DisableShooting();
            float buildTime = towerUpgrades.towerType[(int)currentTowerInfo.towerType].upgradeLevels[upgradeNumber].buildTime;
            currentTowerInfo.gameObject.GetComponent<TowerShooting>().EnableShooting(buildTime);
            UIRadialTimerManager.Instance.AddTimer(currentTowerInfo.transform.position, buildTime);
            selectionWindowItem.PlayerOwns(true);
            UpdateUpgradeStatus();
        }
    }

    public void SellButtonPressed(int moneyBack)
    {
        // give money back from selling a tower
        bank.IncreasePlayerMoney(moneyBack);

        tpg.SelectedTowerWasSold(currentTowerInfo);
    }

    void UpdateNameText()
    {
        towerNameText.text = $"{currentTowerInfo.towerType} L{currentTowerInfo.upgradeIndex + 1}";
    }
}
