using UnityEngine;

public class TowerInfo : MonoBehaviour
{   
    public enum TowerType
    {
        TowerType1 = 0,
        TowerType2 = 1,
        TowerType3 = 2
    }

    public TowerType towerType;

    public int upgradeIndex; // how many times has this tower been upgraded, starts from 0

}
