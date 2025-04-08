using System.Collections.Generic;
using UnityEngine;

public class StoreItemHandler : MonoBehaviour
{
    public StoreHandler storeHandler;
    public Bank bank;

    [Header("Tower Type Upgrades")]
    public TowerTypeUpgradeDataSO towerUpgrades;

    StoreItem[] storeItems; // items that are available in the store

    public void Init()
    {
        storeItems = GetComponentsInChildren<StoreItem>(true);
        foreach(StoreItem storeItem in storeItems)
        {
            // initialize storeItems so that they are ready
            storeItem.Init(towerUpgrades.towerType[(int)storeItem.towerType].upgradeLevels[0].upgradeCost);
        }
        // change the colors of the towers in the store
        ChangeUITowerColors();
    }

    public void ChangeUITowerColors()
    {
        foreach(StoreItem storeItem in storeItems)
        {
            // get the cost of the storeItem (tower) which is the first upgrade cost
            int towerCost = towerUpgrades.towerType[(int)storeItem.towerType].upgradeLevels[0].upgradeCost;

            // change the color of the tower in the store based on if the player has enough money to by them
            if (bank.GetPlayerMoney() >= towerCost)
            {
                storeItem.PlayerCanAfford();
            }
            else
            {
                storeItem.PlayerCannotAfford();
            }
        }
    }
}
