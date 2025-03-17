using UnityEngine;

[System.Serializable]
public class UpgradeLevelData
{
    public int upgradeCost;
    public float range;
    public float attackDamage;
    public float projectileSpeed;
    public float fireRate;
    public float aoeRadius;
}

[CreateAssetMenu(fileName = "TowerTypeUpgradeData", menuName = "Tower Type Upgrade Data")]
public class TowerTypeUpgradeDataSO : ScriptableObject
{
    [Header("Upgrade Levels")]
    public UpgradeLevelData[] upgradeLevels = new UpgradeLevelData[3];
}
