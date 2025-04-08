using UnityEngine;

public class TowerInfo : MonoBehaviour
{   
    
    public enum TowerType
    {
        Basic = 0,
        Sniper = 1,
        AOE = 2
    }

    public TowerType towerType;

    public int upgradeIndex; // how many times has this tower been upgraded, starts from 0

}
