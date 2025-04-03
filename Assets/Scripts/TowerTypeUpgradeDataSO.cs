using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeLevelData
{
    public Sprite towerBaseSprite;
    public Sprite towerTurretSprite;
    public int upgradeCost;
    public float range;
    public float attackDamage;
    public float projectileSpeed;
    public float fireRate;
    public float aoeRadius;
}

[System.Serializable]
public class TowerUpgradeData
{
    public TowerInfo.TowerType Type;
    [Header("Upgrade Levels")]
    public UpgradeLevelData[] upgradeLevels = new UpgradeLevelData[3];
}

[CreateAssetMenu(fileName = "TowerTypeUpgradeData", menuName = "Tower Type Upgrade Data")]
public class TowerTypeUpgradeDataSO : ScriptableObject
{
    public List<TowerUpgradeData> towerType = new List<TowerUpgradeData>();
}
