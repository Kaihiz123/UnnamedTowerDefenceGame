using UnityEngine;

public class TowerUpgrading : MonoBehaviour
{
    private TowerInfo towerInfo;
    private TowerShooting towerShooting;
    private TowerShootingAoE towerShootingAoE;

    private int upgradeIndex;
    private TowerInfo.TowerType towerType;

    private int upgradeCost;
    private float range;
    private float attackDamage;
    private float projectileSpeed;
    private float fireRate;
    private float aoeRadius;

    void Start()
    {
        towerInfo = GetComponent<TowerInfo>();
        towerShooting = GetComponent<TowerShooting>();
        towerShootingAoE = GetComponent<TowerShootingAoE>();

        range = towerShooting.towerEnemyDetectAreaSize;
        attackDamage = towerShooting.projectileAttackDamage;
        projectileSpeed = towerShooting.projectileSpeed;
        fireRate = towerShooting.towerFireRate;

        if (towerShootingAoE != null)
        {
            aoeRadius = towerShootingAoE.projectileAoEAttackRangeRadius;
        }
    }

    public void RunWhenTowerUpgrades()
    {
        towerType = towerInfo.towerType;
        upgradeIndex = towerInfo.upgradeIndex;

        switch ((towerType, upgradeIndex)) // Tuple switch
        {
            case (TowerInfo.TowerType.Basic, 0):
                upgradeCost = 100;
                range = 1f;
                attackDamage = 1f;
                projectileSpeed = 1f;
                fireRate = 1f; // Projectiles per second
                Debug.Log("Basic Tower: First upgrade applied!");
                break;
            case (TowerInfo.TowerType.Basic, 1):
                upgradeCost = 200;
                range = 2f;
                attackDamage = 2f;
                projectileSpeed = 2f;
                fireRate = 2f; // Projectiles per second
                Debug.Log("Basic Tower: Second upgrade applied!");
                break;
            case (TowerInfo.TowerType.Basic, 2):
                upgradeCost = 400;
                range = 4f;
                attackDamage = 4f;
                projectileSpeed = 4f;
                fireRate = 4f; // Projectiles per second
                Debug.Log("Basic Tower: Third upgrade applied!");
                break;

            case (TowerInfo.TowerType.Sniper, 0):
                upgradeCost = 100;
                range = 1f;
                attackDamage = 1f;
                projectileSpeed = 1f;
                fireRate = 1f; // Projectiles per second
                Debug.Log("Sniper Tower: First upgrade applied!");
                break;
            case (TowerInfo.TowerType.Sniper, 1):
                upgradeCost = 200;
                range = 2f;
                attackDamage = 2f;
                projectileSpeed = 2f;
                fireRate = 2f; // Projectiles per second
                Debug.Log("Sniper Tower: Second upgrade applied!");
                break;
            case (TowerInfo.TowerType.Sniper, 2):
                upgradeCost = 400;
                range = 4f;
                attackDamage = 4f;
                projectileSpeed = 4f;
                fireRate = 4f; // Projectiles per second
                Debug.Log("Sniper Tower: Third upgrade applied!");
                break;

            case (TowerInfo.TowerType.AOE, 0):
                upgradeCost = 100;
                range = 1f;
                attackDamage = 1f;
                projectileSpeed = 1f;
                fireRate = 1f; // Projectiles per second
                aoeRadius = 1f;
                Debug.Log("AOE Tower: First upgrade applied!");
                break;
            case (TowerInfo.TowerType.AOE, 1):
                upgradeCost = 200;
                range = 2f;
                attackDamage = 2f;
                projectileSpeed = 2f;
                fireRate = 2f; // Projectiles per second
                aoeRadius = 2f;
                Debug.Log("AOE Tower: Second upgrade applied!");
                break;
            case (TowerInfo.TowerType.AOE, 2):
                upgradeCost = 400;
                range = 4f;
                attackDamage = 4f;
                projectileSpeed = 4f;
                fireRate = 4f; // Projectiles per second
                aoeRadius = 4f;
                Debug.Log("AOE Tower: Third upgrade applied!");
                break;

            default:
                Debug.Log("Unknown tower type or upgrade index!");
                break;
        }

        // Apply the values to the tower

        towerShooting.towerEnemyDetectAreaSize = range;
        towerShooting.projectileAttackDamage = attackDamage;
        towerShooting.projectileSpeed = projectileSpeed;
        towerShooting.towerFireRate = fireRate;
        towerShooting.UpdateFireRate();
        towerShooting.UpdateDebugCircleRadius();

        if (towerShootingAoE != null)
        {
            towerShootingAoE.projectileAoEAttackRangeRadius = aoeRadius;
        }
    }
}

