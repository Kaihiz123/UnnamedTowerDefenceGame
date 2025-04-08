using UnityEngine;
using System.Collections.Generic;

public class StatisticsTracker : MonoBehaviour
{
    // Singleton instance
    public static StatisticsTracker Instance { get; private set; }

    // Dictionaries for various damage stats
    private Dictionary<TowerInfo.TowerType, float> damageByTowerType = new Dictionary<TowerInfo.TowerType, float>();
    private Dictionary<TowerInfo.TowerType, float> shieldBlockedDamageByTowerType = new Dictionary<TowerInfo.TowerType, float>();
    private Dictionary<TowerInfo.TowerType, float> effectiveDamageByTowerType = new Dictionary<TowerInfo.TowerType, float>();
    private Dictionary<TowerInfo.TowerType, float> overkillDamageByTowerType = new Dictionary<TowerInfo.TowerType, float>();
    
    // Wave-specific damage tracking
    private Dictionary<TowerInfo.TowerType, float> currentWaveEffectiveDamage = new Dictionary<TowerInfo.TowerType, float>();
    private Dictionary<TowerInfo.TowerType, float> lastWaveEffectiveDamage = new Dictionary<TowerInfo.TowerType, float>();

    // Dictionary to track kills by enemy type
    private Dictionary<EnemyScript.EnemyType, int> killsByEnemyType = new Dictionary<EnemyScript.EnemyType, int>();

    // Inspector-visible stats (read-only)
    [Header("Current Wave Effective Damage")]
    [SerializeField] private float currentWaveBasicEffectiveDamage;
    [SerializeField] private float currentWaveSniperEffectiveDamage;
    [SerializeField] private float currentWaveAoEEffectiveDamage;
    [Space]
    [SerializeField] private float currentWaveTotalEffectiveDamage;
    
    [Header("Last Wave Effective Damage")]
    [SerializeField] private float lastWaveBasicEffectiveDamage;
    [SerializeField] private float lastWaveSniperEffectiveDamage;
    [SerializeField] private float lastWaveAoEEffectiveDamage;
    [Space]
    [SerializeField] private float lastWaveTotalEffectiveDamage;
    
    [Header("Total Attempted Damage")]
    [SerializeField] private float basicTowerDamage;
    [SerializeField] private float sniperTowerDamage;
    [SerializeField] private float aoeTowerDamage;
    [Space]
    [SerializeField] private float totalDamage;
    
    [Header("Effective Damage")]
    [SerializeField] private float basicTowerEffectiveDamage;
    [SerializeField] private float sniperTowerEffectiveDamage;
    [SerializeField] private float aoeTowerEffectiveDamage;
    [Space]
    [SerializeField] private float totalEffectiveDamage;
    
    [Header("Shield Blocked Damage")]
    [SerializeField] private float basicTowerBlockedDamage;
    [SerializeField] private float sniperTowerBlockedDamage;
    [SerializeField] private float aoeTowerBlockedDamage;
    [Space]
    [SerializeField] private float totalBlockedDamage;
    
    [Header("Overkill Damage")]
    [SerializeField] private float basicTowerOverkillDamage;
    [SerializeField] private float sniperTowerOverkillDamage;
    [SerializeField] private float aoeTowerOverkillDamage;
    [Space]
    [SerializeField] private float totalOverkillDamage;

    [Header("Enemy Kills")]
    [SerializeField] private int basicEnemiesKilled;
    [SerializeField] private int fastEnemiesKilled;
    [SerializeField] private int tankyEnemiesKilled;
    [Space]
    [SerializeField] private int totalEnemiesKilled;

    [Header("Tower Investment")]
    [SerializeField] private float basicTowerInvestment;
    [SerializeField] private float sniperTowerInvestment;
    [SerializeField] private float aoeTowerInvestment;
    [Space]
    [SerializeField] private float totalTowerInvestment;

    private Dictionary<TowerInfo.TowerType, float> investmentByTowerType = new Dictionary<TowerInfo.TowerType, float>();

    [Header("Current Wave Damage Efficiency (Damage/Money)")]
    [SerializeField] private float currentWaveBasicEfficiency;
    [SerializeField] private float currentWaveSniperEfficiency;
    [SerializeField] private float currentWaveAoEEfficiency;
    [Space]
    [SerializeField] private float currentWaveTotalEfficiency;
    
    [Header("Last Wave Damage Efficiency (Damage/Money)")]
    [SerializeField] private float lastWaveBasicEfficiency;
    [SerializeField] private float lastWaveSniperEfficiency;
    [SerializeField] private float lastWaveAoEEfficiency;
    [Space]
    [SerializeField] private float lastWaveTotalEfficiency;
    
    // Calculated efficiency values for current wave
    private Dictionary<TowerInfo.TowerType, float> currentWaveEfficiencyByTowerType = new Dictionary<TowerInfo.TowerType, float>();
    private float currentWaveTotalEfficiencyValue = 0f;
    
    // Stored efficiency values from last wave
    private Dictionary<TowerInfo.TowerType, float> lastWaveEfficiencyByTowerType = new Dictionary<TowerInfo.TowerType, float>();
    private float lastWaveTotalEfficiencyValue = 0f;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize dictionaries with all tower types
        foreach (TowerInfo.TowerType towerType in System.Enum.GetValues(typeof(TowerInfo.TowerType)))
        {
            damageByTowerType[towerType] = 0f;
            shieldBlockedDamageByTowerType[towerType] = 0f;
            effectiveDamageByTowerType[towerType] = 0f;
            overkillDamageByTowerType[towerType] = 0f;
            currentWaveEffectiveDamage[towerType] = 0f;
            lastWaveEffectiveDamage[towerType] = 0f;
            investmentByTowerType[towerType] = 0f;
            currentWaveEfficiencyByTowerType[towerType] = 0f;
            lastWaveEfficiencyByTowerType[towerType] = 0f;
        }
        
        // Initialize kills dictionary with all enemy types
        foreach (EnemyScript.EnemyType enemyType in System.Enum.GetValues(typeof(EnemyScript.EnemyType)))
        {
            killsByEnemyType[enemyType] = 0;
        }
    }

    // Record damage attempt by a specific tower type
    public void RecordDamageAttempt(TowerInfo.TowerType towerType, float damageAmount)
    {
        if (damageByTowerType.ContainsKey(towerType))
        {
            damageByTowerType[towerType] += damageAmount;
        }
        else
        {
            damageByTowerType[towerType] = damageAmount;
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }
    
    // Record damage blocked by shields
    public void RecordShieldBlockedDamage(TowerInfo.TowerType towerType, float damageAmount)
    {
        if (shieldBlockedDamageByTowerType.ContainsKey(towerType))
        {
            shieldBlockedDamageByTowerType[towerType] += damageAmount;
        }
        else
        {
            shieldBlockedDamageByTowerType[towerType] = damageAmount;
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }
    
    // Record effective damage (actual health removed)
    public void RecordEffectiveDamage(TowerInfo.TowerType towerType, float damageAmount)
    {
        // Record in total effective damage
        if (effectiveDamageByTowerType.ContainsKey(towerType))
        {
            effectiveDamageByTowerType[towerType] += damageAmount;
        }
        else
        {
            effectiveDamageByTowerType[towerType] = damageAmount;
        }
        
        // Also record in current wave damage
        if (currentWaveEffectiveDamage.ContainsKey(towerType))
        {
            currentWaveEffectiveDamage[towerType] += damageAmount;
        }
        else
        {
            currentWaveEffectiveDamage[towerType] = damageAmount;
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }
    
    // Record overkill damage (damage beyond enemy's health)
    public void RecordOverkillDamage(TowerInfo.TowerType towerType, float damageAmount)
    {
        if (overkillDamageByTowerType.ContainsKey(towerType))
        {
            overkillDamageByTowerType[towerType] += damageAmount;
        }
        else
        {
            overkillDamageByTowerType[towerType] = damageAmount;
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }

    // Getter methods for all damage types
    public float GetTotalDamageByTowerType(TowerInfo.TowerType towerType)
    {
        if (damageByTowerType.ContainsKey(towerType))
        {
            return damageByTowerType[towerType];
        }
        return 0f;
    }
    
    public float GetTotalShieldBlockedDamageByTowerType(TowerInfo.TowerType towerType)
    {
        if (shieldBlockedDamageByTowerType.ContainsKey(towerType))
        {
            return shieldBlockedDamageByTowerType[towerType];
        }
        return 0f;
    }
    
    public float GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType towerType)
    {
        if (effectiveDamageByTowerType.ContainsKey(towerType))
        {
            return effectiveDamageByTowerType[towerType];
        }
        return 0f;
    }
    
    public float GetTotalOverkillDamageByTowerType(TowerInfo.TowerType towerType)
    {
        if (overkillDamageByTowerType.ContainsKey(towerType))
        {
            return overkillDamageByTowerType[towerType];
        }
        return 0f;
    }

    // Get total amounts across all tower types
    public float GetTotalDamage()
    {
        float totalDamage = 0f;
        foreach (var damage in damageByTowerType.Values)
        {
            totalDamage += damage;
        }
        return totalDamage;
    }
    
    public float GetTotalShieldBlockedDamage()
    {
        float totalBlockedDamage = 0f;
        foreach (var damage in shieldBlockedDamageByTowerType.Values)
        {
            totalBlockedDamage += damage;
        }
        return totalBlockedDamage;
    }
    
    public float GetTotalEffectiveDamage()
    {
        float total = 0f;
        foreach (var damage in effectiveDamageByTowerType.Values)
        {
            total += damage;
        }
        return total;
    }
    
    public float GetTotalOverkillDamage()
    {
        float total = 0f;
        foreach (var damage in overkillDamageByTowerType.Values)
        {
            total += damage;
        }
        return total;
    }

    // Flag stats for reset
    private bool statisticsResetRequested = false;

    // Public method to request statistics reset
    public void RequestStatisticsReset()
    {
        statisticsResetRequested = true;
    }

    // Reset all statistics
    public void ResetStatistics()
    {
        // Create a safe copy of the tower type keys to avoid enumeration issues
        List<TowerInfo.TowerType> towerTypesList = new List<TowerInfo.TowerType>(damageByTowerType.Keys);
        
        foreach (TowerInfo.TowerType towerType in towerTypesList)
        {
            // Reset damage statistics
            damageByTowerType[towerType] = 0f;
            shieldBlockedDamageByTowerType[towerType] = 0f;
            effectiveDamageByTowerType[towerType] = 0f;
            overkillDamageByTowerType[towerType] = 0f;
            
            // Reset wave damage statistics
            currentWaveEffectiveDamage[towerType] = 0f;
            lastWaveEffectiveDamage[towerType] = 0f;
            
            // Reset investment data
            investmentByTowerType[towerType] = 0f;
            
            // Reset efficiency metrics
            currentWaveEfficiencyByTowerType[towerType] = 0f;
            lastWaveEfficiencyByTowerType[towerType] = 0f;
        }
        
        // Reset total efficiency values
        currentWaveTotalEfficiencyValue = 0f;
        lastWaveTotalEfficiencyValue = 0f;
        
        // Reset kill statistics
        List<EnemyScript.EnemyType> enemyTypesList = new List<EnemyScript.EnemyType>(killsByEnemyType.Keys);
        foreach (EnemyScript.EnemyType enemyType in enemyTypesList)
        {
            killsByEnemyType[enemyType] = 0;
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }

    // Dictionary getters
    public Dictionary<TowerInfo.TowerType, float> GetAllDamageStats()
    {
        return new Dictionary<TowerInfo.TowerType, float>(damageByTowerType);
    }
    
    public Dictionary<TowerInfo.TowerType, float> GetAllShieldBlockedDamageStats()
    {
        return new Dictionary<TowerInfo.TowerType, float>(shieldBlockedDamageByTowerType);
    }
    
    public Dictionary<TowerInfo.TowerType, float> GetAllEffectiveDamageStats()
    {
        return new Dictionary<TowerInfo.TowerType, float>(effectiveDamageByTowerType);
    }
    
    public Dictionary<TowerInfo.TowerType, float> GetAllOverkillDamageStats()
    {
        return new Dictionary<TowerInfo.TowerType, float>(overkillDamageByTowerType);
    }
    
    // Called when a new wave starts
    public void OnWaveStart()
    {
        // Reset current wave damage stats
        foreach (TowerInfo.TowerType towerType in System.Enum.GetValues(typeof(TowerInfo.TowerType)))
        {
            currentWaveEffectiveDamage[towerType] = 0f;
        }
        
        UpdateInspectorValues();
    }
    
    // Calculate and store current wave efficiency values
    private void UpdateCurrentWaveEfficiency()
    {
        // Calculate for each tower type
        foreach (TowerInfo.TowerType towerType in System.Enum.GetValues(typeof(TowerInfo.TowerType)))
        {
            float investment = GetInvestmentByTowerType(towerType);
            float damage = GetCurrentWaveEffectiveDamage(towerType);
            
            currentWaveEfficiencyByTowerType[towerType] = investment <= 0 ? 0 : damage / investment;
        }
        
        // Calculate total
        float totalInvestment = GetTotalTowerInvestment();
        float totalDamage = GetCurrentWaveTotalEffectiveDamage();
        
        currentWaveTotalEfficiencyValue = totalInvestment <= 0 ? 0 : totalDamage / totalInvestment;
    }
    
    // Called when a wave ends
    public void OnWaveEnd()
    {
        // First make sure current wave efficiency is up to date
        UpdateCurrentWaveEfficiency();
        
        // Move current wave stats to last wave stats
        foreach (TowerInfo.TowerType towerType in System.Enum.GetValues(typeof(TowerInfo.TowerType)))
        {
            lastWaveEffectiveDamage[towerType] = currentWaveEffectiveDamage[towerType];
            
            // Copy current efficiency values to last wave
            lastWaveEfficiencyByTowerType[towerType] = currentWaveEfficiencyByTowerType[towerType];
        }
        
        // Copy total efficiency
        lastWaveTotalEfficiencyValue = currentWaveTotalEfficiencyValue;
        
        UpdateInspectorValues();
    }

    // Get total current wave effective damage
    public float GetCurrentWaveTotalEffectiveDamage()
    {
        float total = 0f;
        foreach (var damageValue in currentWaveEffectiveDamage.Values)
        {
            total += damageValue;
        }
        return total;
    }
    
    // Get total last wave effective damage
    public float GetLastWaveTotalEffectiveDamage()
    {
        float total = 0f;
        foreach (var damageValue in lastWaveEffectiveDamage.Values)
        {
            total += damageValue;
        }
        return total;
    }
    
    // Get current wave effective damage for a specific tower type
    public float GetCurrentWaveEffectiveDamage(TowerInfo.TowerType towerType)
    {
        if (currentWaveEffectiveDamage.ContainsKey(towerType))
        {
            return currentWaveEffectiveDamage[towerType];
        }
        return 0f;
    }
    
    // Get last wave effective damage for a specific tower type
    public float GetLastWaveEffectiveDamage(TowerInfo.TowerType towerType)
    {
        if (lastWaveEffectiveDamage.ContainsKey(towerType))
        {
            return lastWaveEffectiveDamage[towerType];
        }
        return 0f;
    }
    
    // Record an enemy kill by type
    public void RecordEnemyKill(EnemyScript.EnemyType enemyType)
    {
        if (killsByEnemyType.ContainsKey(enemyType))
        {
            killsByEnemyType[enemyType]++;
        }
        else
        {
            killsByEnemyType[enemyType] = 1;
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }

    // Get total kills for a specific enemy type
    public int GetKillsByEnemyType(EnemyScript.EnemyType enemyType)
    {
        if (killsByEnemyType.ContainsKey(enemyType))
        {
            return killsByEnemyType[enemyType];
        }
        return 0;
    }

    // Get total kills across all enemy types
    public int GetTotalEnemyKills()
    {
        int totalKills = 0;
        foreach (var kills in killsByEnemyType.Values)
        {
            totalKills += kills;
        }
        return totalKills;
    }
    
    // Flag to indicate recalculation is needed
    private bool recalculationNeeded = false;
    
    // Public method to request recalculation
    public void RequestTowerInvestmentRecalculation()
    {
        recalculationNeeded = true;
    }
    
    // LateUpdate runs after all Update methods are called
    private void LateUpdate()
    {
        // Only recalculate if needed
        if (recalculationNeeded)
        {
            RecalculateTowerInvestment();
            recalculationNeeded = false;
        }

        // Handle statistics reset if requested
        if (statisticsResetRequested)
        {
            ResetStatistics();
            statisticsResetRequested = false;
        }
    }
    
    // Method to recalculate total investment in all towers
    public void RecalculateTowerInvestment()
    {
        // Reset investment counts
        foreach (TowerInfo.TowerType towerType in System.Enum.GetValues(typeof(TowerInfo.TowerType)))
        {
            investmentByTowerType[towerType] = 0f;
        }

        // Find all towers in the scene
        TowerInfo[] allTowers = GameObject.FindObjectsByType<TowerInfo>(FindObjectsSortMode.None);
        
        foreach (TowerInfo tower in allTowers)
        {
            TowerUpgrading towerUpgrading = tower.GetComponent<TowerUpgrading>();
            if (towerUpgrading != null && towerUpgrading.towerUpgrades != null)
            {
                // Calculate total investment for this tower (initial cost + all upgrades)
                float towerInvestment = 0f;
                
                // Get the tower type and current upgrade level
                TowerInfo.TowerType towerType = tower.towerType;
                int upgradeIndex = tower.upgradeIndex;
                
                // Find the initial cost and all upgrade costs up to the current level
                TowerUpgradeData upgradeData = towerUpgrading.towerUpgrades.towerType.Find(t => t.Type == towerType);
                if (upgradeData != null)
                {
                    // Add cost for each upgrade level up to the current one
                    for (int i = 0; i <= upgradeIndex && i < upgradeData.upgradeLevels.Length; i++)
                    {
                        towerInvestment += upgradeData.upgradeLevels[i].upgradeCost;
                    }
                    
                    // Add to the investment for this tower type
                    investmentByTowerType[towerType] += towerInvestment;
                }
            }
        }
        
        // Update inspector values
        UpdateInspectorValues();
    }

    // Get total investment for a specific tower type
    public float GetInvestmentByTowerType(TowerInfo.TowerType towerType)
    {
        if (investmentByTowerType.ContainsKey(towerType))
        {
            return investmentByTowerType[towerType];
        }
        return 0f;
    }

    // Get total investment across all tower types
    public float GetTotalTowerInvestment()
    {
        float total = 0f;
        foreach (var investment in investmentByTowerType.Values)
        {
            total += investment;
        }
        return total;
    }
    
    // Getter methods for efficiency values
    public float GetCurrentWaveDamageEfficiency(TowerInfo.TowerType towerType)
    {
        return currentWaveEfficiencyByTowerType.ContainsKey(towerType) ? 
               currentWaveEfficiencyByTowerType[towerType] : 0f;
    }
    
    public float GetLastWaveDamageEfficiency(TowerInfo.TowerType towerType)
    {
        return lastWaveEfficiencyByTowerType.ContainsKey(towerType) ? 
               lastWaveEfficiencyByTowerType[towerType] : 0f;
    }
    
    public float GetCurrentWaveTotalDamageEfficiency()
    {
        return currentWaveTotalEfficiencyValue;
    }
    
    public float GetLastWaveTotalDamageEfficiency()
    {
        return lastWaveTotalEfficiencyValue;
    }
    
    // Update the inspector-visible values
    private void UpdateInspectorValues()
    {
        
        // Update damage attempts
        basicTowerDamage = GetTotalDamageByTowerType(TowerInfo.TowerType.Basic);
        sniperTowerDamage = GetTotalDamageByTowerType(TowerInfo.TowerType.Sniper);
        aoeTowerDamage = GetTotalDamageByTowerType(TowerInfo.TowerType.AOE);
        totalDamage = GetTotalDamage();
        
        // Update shield blocks
        basicTowerBlockedDamage = GetTotalShieldBlockedDamageByTowerType(TowerInfo.TowerType.Basic);
        sniperTowerBlockedDamage = GetTotalShieldBlockedDamageByTowerType(TowerInfo.TowerType.Sniper);
        aoeTowerBlockedDamage = GetTotalShieldBlockedDamageByTowerType(TowerInfo.TowerType.AOE);
        totalBlockedDamage = GetTotalShieldBlockedDamage();
        
        // Update effective damage
        basicTowerEffectiveDamage = GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType.Basic);
        sniperTowerEffectiveDamage = GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType.Sniper);
        aoeTowerEffectiveDamage = GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType.AOE);
        totalEffectiveDamage = GetTotalEffectiveDamage();
        
        // Update overkill damage
        basicTowerOverkillDamage = GetTotalOverkillDamageByTowerType(TowerInfo.TowerType.Basic);
        sniperTowerOverkillDamage = GetTotalOverkillDamageByTowerType(TowerInfo.TowerType.Sniper);
        aoeTowerOverkillDamage = GetTotalOverkillDamageByTowerType(TowerInfo.TowerType.AOE);
        totalOverkillDamage = GetTotalOverkillDamage();
        
        // Update current wave statistics
        currentWaveBasicEffectiveDamage = GetCurrentWaveEffectiveDamage(TowerInfo.TowerType.Basic);
        currentWaveSniperEffectiveDamage = GetCurrentWaveEffectiveDamage(TowerInfo.TowerType.Sniper);
        currentWaveAoEEffectiveDamage = GetCurrentWaveEffectiveDamage(TowerInfo.TowerType.AOE);
        currentWaveTotalEffectiveDamage = GetCurrentWaveTotalEffectiveDamage();
        
        // Update last wave statistics
        lastWaveBasicEffectiveDamage = GetLastWaveEffectiveDamage(TowerInfo.TowerType.Basic);
        lastWaveSniperEffectiveDamage = GetLastWaveEffectiveDamage(TowerInfo.TowerType.Sniper);
        lastWaveAoEEffectiveDamage = GetLastWaveEffectiveDamage(TowerInfo.TowerType.AOE);
        lastWaveTotalEffectiveDamage = GetLastWaveTotalEffectiveDamage();
        
        // Update kill statistics
        basicEnemiesKilled = GetKillsByEnemyType(EnemyScript.EnemyType.Basic);
        fastEnemiesKilled = GetKillsByEnemyType(EnemyScript.EnemyType.Fast);
        tankyEnemiesKilled = GetKillsByEnemyType(EnemyScript.EnemyType.Tanky);
        totalEnemiesKilled = GetTotalEnemyKills();

        // Update tower investment
        basicTowerInvestment = GetInvestmentByTowerType(TowerInfo.TowerType.Basic);
        sniperTowerInvestment = GetInvestmentByTowerType(TowerInfo.TowerType.Sniper);
        aoeTowerInvestment = GetInvestmentByTowerType(TowerInfo.TowerType.AOE);
        totalTowerInvestment = GetTotalTowerInvestment();
        
        // Make sure efficiency values are up to date
        UpdateCurrentWaveEfficiency();
        
        // Update efficiency values in inspector
        currentWaveBasicEfficiency = GetCurrentWaveDamageEfficiency(TowerInfo.TowerType.Basic);
        currentWaveSniperEfficiency = GetCurrentWaveDamageEfficiency(TowerInfo.TowerType.Sniper);
        currentWaveAoEEfficiency = GetCurrentWaveDamageEfficiency(TowerInfo.TowerType.AOE);
        currentWaveTotalEfficiency = GetCurrentWaveTotalDamageEfficiency();
        
        lastWaveBasicEfficiency = GetLastWaveDamageEfficiency(TowerInfo.TowerType.Basic);
        lastWaveSniperEfficiency = GetLastWaveDamageEfficiency(TowerInfo.TowerType.Sniper);
        lastWaveAoEEfficiency = GetLastWaveDamageEfficiency(TowerInfo.TowerType.AOE);
        lastWaveTotalEfficiency = GetLastWaveTotalDamageEfficiency();
    }
}
