using UnityEngine;

public class TowerUpgrading : MonoBehaviour
{
    private TowerInfo towerInfo;
    private TowerShooting towerShooting;
    private TowerShootingAoE towerShootingAoE;

    [Header("Tower Type Upgrades")]
    public TowerTypeUpgradeDataSO towerUpgrades;

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

        // get upgrade information from scriptable object
        upgradeData = towerUpgrades.towerType[(int)towerType].upgradeLevels[upgradeIndex];

        if (upgradeData != null)
        {
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