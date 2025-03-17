using UnityEngine;

public class TowerUpgrading : MonoBehaviour
{
    private TowerInfo towerInfo;
    private TowerShooting towerShooting;
    private TowerShootingAoE towerShootingAoE;

    [Header("Tower Type Upgrades")]
    public TowerTypeUpgradeDataSO basicTowerUpgrades;
    public TowerTypeUpgradeDataSO sniperTowerUpgrades;
    public TowerTypeUpgradeDataSO aoeTowerUpgrades;
    public int upgradeCost { get; private set; } // Keep this public in case store needs to access it

    void Start()
    {
        towerInfo = GetComponent<TowerInfo>();
        towerShooting = GetComponent<TowerShooting>();
        towerShootingAoE = GetComponent<TowerShootingAoE>();
    }

    public void RunWhenTowerUpgrades()
    {
        TowerInfo.TowerType towerType = towerInfo.towerType;
        int upgradeIndex = towerInfo.upgradeIndex;
        UpgradeLevelData upgradeData = null;

        switch ((towerType, upgradeIndex)) // Tuple switch
        {
            case (TowerInfo.TowerType.Basic, 0):
                upgradeData = basicTowerUpgrades.upgradeLevels[0];
                Debug.Log("Basic Tower: First upgrade applied!");
                break;
            case (TowerInfo.TowerType.Basic, 1):
                upgradeData = basicTowerUpgrades.upgradeLevels[1];
                Debug.Log("Basic Tower: Second upgrade applied!");
                break;
            case (TowerInfo.TowerType.Basic, 2):
                upgradeData = basicTowerUpgrades.upgradeLevels[2];
                Debug.Log("Basic Tower: Third upgrade applied!");
                break;

            case (TowerInfo.TowerType.Sniper, 0):
                upgradeData = sniperTowerUpgrades.upgradeLevels[0];
                Debug.Log("Sniper Tower: First upgrade applied!");
                break;
            case (TowerInfo.TowerType.Sniper, 1):
                upgradeData = sniperTowerUpgrades.upgradeLevels[1];
                Debug.Log("Sniper Tower: Second upgrade applied!");
                break;
            case (TowerInfo.TowerType.Sniper, 2):
                upgradeData = sniperTowerUpgrades.upgradeLevels[2];
                Debug.Log("Sniper Tower: Third upgrade applied!");
                break;

            case (TowerInfo.TowerType.AOE, 0):
                upgradeData = aoeTowerUpgrades.upgradeLevels[0];
                Debug.Log("AOE Tower: First upgrade applied!");
                break;
            case (TowerInfo.TowerType.AOE, 1):
                upgradeData = aoeTowerUpgrades.upgradeLevels[1];
                Debug.Log("AOE Tower: Second upgrade applied!");
                break;
            case (TowerInfo.TowerType.AOE, 2):
                upgradeData = aoeTowerUpgrades.upgradeLevels[2];
                Debug.Log("AOE Tower: Third upgrade applied!");
                break;

            default:
                Debug.Log("Unknown tower type or upgrade index!");
                return;
        }

        if (upgradeData != null)
        {
            // Store the upgrade cost in a public property
            upgradeCost = upgradeData.upgradeCost;
            
            // Apply the values directly to the tower
            towerShooting.towerEnemyDetectAreaSize = upgradeData.range;
            towerShooting.projectileAttackDamage = upgradeData.attackDamage;
            towerShooting.projectileSpeed = upgradeData.projectileSpeed;
            towerShooting.towerFireRate = upgradeData.fireRate;
            towerShooting.UpdateFireRate();
            towerShooting.UpdateDebugCircleRadius();

            if (towerShootingAoE != null)
            {
                towerShootingAoE.projectileAoEAttackRangeRadius = upgradeData.aoeRadius;
            }
        }
    }
}