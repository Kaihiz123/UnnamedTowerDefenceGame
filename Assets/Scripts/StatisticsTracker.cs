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
    
    [Header("Total Attempted Damage (Read-Only)")]
    [SerializeField] private float basicTowerDamage;
    [SerializeField] private float sniperTowerDamage;
    [SerializeField] private float aoeTowerDamage;
    [Space]
    [SerializeField] private float totalDamage;
    
    [Header("Effective Damage (Read-Only)")]
    [SerializeField] private float basicTowerEffectiveDamage;
    [SerializeField] private float sniperTowerEffectiveDamage;
    [SerializeField] private float aoeTowerEffectiveDamage;
    [Space]
    [SerializeField] private float totalEffectiveDamage;
    
    [Header("Shield Blocked Damage (Read-Only)")]
    [SerializeField] private float basicTowerBlockedDamage;
    [SerializeField] private float sniperTowerBlockedDamage;
    [SerializeField] private float aoeTowerBlockedDamage;
    [Space]
    [SerializeField] private float totalBlockedDamage;
    
    [Header("Overkill Damage (Read-Only)")]
    [SerializeField] private float basicTowerOverkillDamage;
    [SerializeField] private float sniperTowerOverkillDamage;
    [SerializeField] private float aoeTowerOverkillDamage;
    [Space]
    [SerializeField] private float totalOverkillDamage;

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

    // Reset all statistics
    public void ResetStatistics()
    {
        foreach (TowerInfo.TowerType towerType in damageByTowerType.Keys)
        {
            damageByTowerType[towerType] = 0f;
            shieldBlockedDamageByTowerType[towerType] = 0f;
            effectiveDamageByTowerType[towerType] = 0f;
            overkillDamageByTowerType[towerType] = 0f;
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
    
    // Called when a wave ends
    public void OnWaveEnd()
    {
        // Move current wave stats to last wave stats
        foreach (TowerInfo.TowerType towerType in System.Enum.GetValues(typeof(TowerInfo.TowerType)))
        {
            lastWaveEffectiveDamage[towerType] = currentWaveEffectiveDamage[towerType];
        }
        
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
    }
}
