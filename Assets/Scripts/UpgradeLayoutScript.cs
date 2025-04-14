using UnityEngine;

public class UpgradeLayoutScript : MonoBehaviour
{
    public TowerInfo.TowerType towerType;
    
    public TMPro.TextMeshProUGUI upgrade1ButtonText;
    public TMPro.TextMeshProUGUI upgrade2ButtonText;
    public TMPro.TextMeshProUGUI upgrade3ButtonText;
    public TMPro.TextMeshProUGUI sellButtonText;

    [Header("Tower Type Upgrades")]
    public TowerTypeUpgradeDataSO towerUpgrades;

    TowerInfo currentTowerInfo;

    SelectionWindow selectionWindow;

    public void Init(TowerInfo towerInfo, SelectionWindow selectionWindow)
    {
        currentTowerInfo = towerInfo;
        this.selectionWindow = selectionWindow;
        UpdateTexts();
    }

    public void UpdateTexts()
    {
        upgrade1ButtonText.text = "" + towerUpgrades.towerType[(int)towerType].upgradeLevels[0].upgradeCost;
        upgrade2ButtonText.text = "" + towerUpgrades.towerType[(int)towerType].upgradeLevels[1].upgradeCost;
        upgrade3ButtonText.text = "" + towerUpgrades.towerType[(int)towerType].upgradeLevels[2].upgradeCost;

        int moneyBack = 0;
        for (int i = 0; i <= currentTowerInfo.upgradeIndex; i++)
        {
            moneyBack += towerUpgrades.towerType[(int)towerType].upgradeLevels[i].upgradeCost;
        }
        moneyBack /= 2;

        sellButtonText.text = "" + moneyBack;
    }

    public void MouseButtonDown(GameObject go)
    {

    }

    public void MouseButtonUp(GameObject go, int upgradeIndex, SelectionWindowItem selectionWindowItem)
    {
        // confirm that mouse button was pressed down and up on the same button
        if (hoverOverButton == go)
        {
            // -1 -> we are selling the tower
            // >0 -> we are upgrading (when tower is bought it is upgraded to first level)
            if (upgradeIndex == -1)
            {
                // give player only half the money back
                int moneyBack = 0;
                for (int i = 0; i <= currentTowerInfo.upgradeIndex; i++)
                {
                    moneyBack += towerUpgrades.towerType[(int)currentTowerInfo.towerType].upgradeLevels[i].upgradeCost;
                }
                moneyBack /= 2;

                selectionWindow.SellButtonPressed(moneyBack);
            }
            else
            {
                // Check that this upgrade haven't been bought already. Also prevent player skipping upgrades.
                if (currentTowerInfo.upgradeIndex + 1 == upgradeIndex)
                {
                    selectionWindow.UpgradeButtonPressed(upgradeIndex, towerUpgrades.towerType[(int)towerType].upgradeLevels[upgradeIndex].upgradeCost, selectionWindowItem);
                }
            }

            UpdateTexts();
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
