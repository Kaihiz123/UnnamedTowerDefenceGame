using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button nextWaveButton;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private bool nextWaveTriggered = false; // visible for testing
    [SerializeField] private int helperCurrentWave = 0;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private int startFromWave = 1;
    [SerializeField] private bool useRandomRepeatingWaves = true;
    [SerializeField] private int selectedRepeatingWaveIndex = 0;
    
    [Header("Bonus Settings")]
    [SerializeField] private int waveBonusInterval = 5;
    [SerializeField] private int waveBonusAmount = 200;
    [SerializeField] private bool enableWaveBonus = true;

    
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private GameObject waypointsParent;
    [SerializeField] private PlayerHealthSystem playerHealthSystem;
    [SerializeField] private Bank bank;

    [Header("Infinite Wave Settings")]
    [SerializeField] private float infiniteWaveScalingIncrement = 0.1f;
    [SerializeField] private float maxScalingMultiplier = 5f;
    [SerializeField] private int globalInfiniteWaveReward = 100;
    [SerializeField] private bool useGlobalInfiniteWaveReward = true;
    
    // Defines the different types of enemies that can be spawned
    public enum EnemyType
    {
        [InspectorName("Basic Enemy")] BasicEnemy,
        [InspectorName("Fast Enemy")] FastEnemy,
        [InspectorName("Tank Enemy")] TankEnemy,
        [InspectorName("Delay")] Delay
    }

    // Represents a single enemy spawn configuration
    [System.Serializable]
    public class EnemySpawn
    {
        public EnemyType enemyType;
        // Delay before spawning the next enemy in seconds
        public float spawnDelay;
        // Number of this enemy type to spawn consecutively
        public int count = 1;
        // Number of shield charges this enemy has
        public int shieldCharges = 0;
    }

    // Represents a complete wave of enemies
    [System.Serializable]
    public class Wave
    {
        public int waveReward = 0;
        public float enemyScaling = 1f;
        public List<EnemySpawn> enemies = new List<EnemySpawn>();
    }

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject basicEnemy;
    [SerializeField] private GameObject fastEnemy;
    [SerializeField] private GameObject tankEnemy;
    
    [Header("Wave Configuration")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private List<Wave> repeatingWaves = new List<Wave>();
    
    // Reference to the current active wave
    private Wave currentActiveWave;
    // To track the scaling for infinite waves
    private float currentScalingMultiplier = 1f;
    private int infiniteWavesCompleted = 0;
    private Cleanup cleanup;

    // Returns the correct enemy prefab based on the enemy type
    private GameObject GetEnemyPrefab(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.BasicEnemy:
                return basicEnemy;
            case EnemyType.FastEnemy:
                return fastEnemy;
            case EnemyType.TankEnemy:
                return tankEnemy;
            case EnemyType.Delay:
                return null;
            default:
                return basicEnemy;
        }
    }

    public int GetWavesReached()
    {
        return helperCurrentWave;
    }

    void Start()
    {
        // if bank reference is not set, find it in the scene
        if (bank == null)
        {
            bank = FindFirstObjectByType<Bank>();
        }

        // if enemies parent is not set, find it in the scene by name
        if (enemyParent == null)
        {
            enemyParent = GameObject.Find("EnemiesParent").transform;
        }

        cleanup = new Cleanup();
        UpdateWaveText(1);
        StartCoroutine(Spawning());
    }

    private void UpdateWaveText(int waveNumber)
    {
        if (waveText != null)
        {
                waveText.text = $"Wave: {waveNumber}";
        }
    }

    public void DisableWaveText()
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(false);
        }
    }

    // Add this new method near the other utility methods
    private int CalculateIncomeRaise(int waveNumber)
    {
        // +10 money for each completed 10 waves
        return (waveNumber / 10) * 10;
    }

    // Tää on ihan hirvee mörkö minkä vois pilkkoo osiin jos uskaltaa
    // Main spawning coroutine that handles wave progression
    public IEnumerator Spawning()
    {
        // Start from the specified wave in debug mode
        int startWaveIndex = debugMode ? startFromWave -1 : 0;
            // Calculate bank balance for skipped waves if debug mode is enabled
            if (debugMode && startWaveIndex > 0)
            {
                int accumulatedMoney = 0;
                    // Calculate money from main waves
                    for (int i = 0; i < Mathf.Min(startWaveIndex, waves.Count); i++)
                    {
                        int waveNumber = i + 1;
                        int baseReward = waves[i].waveReward;
                        int incomeRaise = CalculateIncomeRaise(waveNumber);
                    
                        accumulatedMoney += baseReward + incomeRaise;
                    
                        if (incomeRaise > 0)
                        {
                            Debug.Log($"Debug mode: Added income raise of {incomeRaise} for wave {waveNumber}");
                        }
                    
                        // Add wave bonus if applicable
                        if (enableWaveBonus && waveNumber % waveBonusInterval == 0)
                        {
                            accumulatedMoney += waveBonusAmount;
                            Debug.Log($"Debug mode: Added wave bonus of {waveBonusAmount} for wave {waveNumber}");
                        }
                }
            
                // Calculate money for repeating waves
                if (startWaveIndex > waves.Count && repeatingWaves.Count > 0)
                {
                    int repeatingWavesSkipped = startWaveIndex - waves.Count;
                    
                    // Initialize infinite waves completed and scaling multiplier when starting from repeating waves
                    infiniteWavesCompleted = repeatingWavesSkipped;
                    currentScalingMultiplier = Mathf.Min(
                        1f + (infiniteWavesCompleted * infiniteWaveScalingIncrement), 
                        maxScalingMultiplier
                    );
                    
                    Debug.Log($"Debug mode: Starting at infinite wave {infiniteWavesCompleted} with scaling multiplier {currentScalingMultiplier:F2}");
                    
                    for (int waveNumber = waves.Count + 1; waveNumber <= waves.Count + repeatingWavesSkipped; waveNumber++)
                    {
                        // Add base infinite wave reward
                        accumulatedMoney += globalInfiniteWaveReward;
                        
                        // Add income raise
                        int incomeRaise = CalculateIncomeRaise(waveNumber);
                        if (incomeRaise > 0)
                        {
                            accumulatedMoney += incomeRaise;
                            Debug.Log($"Debug mode: Added income raise of {incomeRaise} for infinite wave {waveNumber}");
                        }
                        
                        // Add wave bonuses if applicable
                        if (enableWaveBonus && waveNumber % waveBonusInterval == 0)
                        {
                            accumulatedMoney += waveBonusAmount;
                        }
                    }
                    
                    Debug.Log($"Debug mode: Added rewards for {repeatingWavesSkipped} skipped repeating waves");
                }            
                // Add the accumulated money to the player's bank
                if (accumulatedMoney > 0)
                {
                    bank.IncreasePlayerMoney(accumulatedMoney);
                    Debug.Log($"Debug mode: Added {accumulatedMoney} total money for skipped waves (1-{startWaveIndex})");
                }
            }
        int waveIndex = startWaveIndex;
        
        while (true)
        {
            if (waveIndex < waves.Count)
            {
                // Wait for player to trigger wave (only for main waves)
                nextWaveButton.gameObject.SetActive(true);
                UpdateWaveText(waveIndex + 1);
                yield return new WaitUntil(() => nextWaveTriggered);
                nextWaveButton.gameObject.SetActive(false);
                nextWaveTriggered = false;
            }
            else
            {
                // No button for repeating waves
                yield return new WaitForSeconds(2f);
            }
            
            // Get the current wave
            if (waveIndex < waves.Count)
            {
                // Still in main wave list
                currentActiveWave = waves[waveIndex];
            }
            else
            {
                // Switched to repeating waves
                if (waveIndex == waves.Count)
                {
                    Debug.Log("Main waves completed! Starting repeating waves.");
                    
                    if (repeatingWaves.Count == 0)
                    {
                        Debug.Log("No repeating waves defined. Game over!");
                        yield break;
                    }
                    
                    // Reset for first infinite wave
                    infiniteWavesCompleted = 0;
                    currentScalingMultiplier = 1f;
                }
                
                int randomIndex;

                if (useRandomRepeatingWaves)
                {
                    // Select a random wave from repeating waves
                    randomIndex = Random.Range(0, repeatingWaves.Count);
                }
                else
                {
                    // Use selected wave from the repeating pool for testing
                    randomIndex = Mathf.Clamp(selectedRepeatingWaveIndex, 0, repeatingWaves.Count - 1);
                }

                currentActiveWave = repeatingWaves[randomIndex];

                // Apply scaling multiplier to the infinite wave
                // Clone of the wave to modify its scaling without affecting the original
                Wave modifiedWave = new Wave();
                
                // Use global reward for infinite waves if enabled
                modifiedWave.waveReward = useGlobalInfiniteWaveReward ? globalInfiniteWaveReward : currentActiveWave.waveReward;
                modifiedWave.enemies = currentActiveWave.enemies;
                
                // Apply the scaling multiplier to the base scaling of the wave
                modifiedWave.enemyScaling = currentActiveWave.enemyScaling * currentScalingMultiplier;
                
                // Set the modified wave as current active wave
                currentActiveWave = modifiedWave;
                
                if (useRandomRepeatingWaves)
                {
                    Debug.Log($"Selected wave pattern {randomIndex + 1} with scaling multiplier {currentScalingMultiplier:F2} and reward {modifiedWave.waveReward}");
                }
                else
                {
                    Debug.Log($"Forced wave pattern {randomIndex + 1} with scaling multiplier {currentScalingMultiplier:F2} and reward {modifiedWave.waveReward}");
                }           
            }
            
            helperCurrentWave = waveIndex + 1; // update current wave for inspector
            UpdateWaveText(helperCurrentWave);
            
            // Check if the wave is empty
            if (currentActiveWave.enemies == null || currentActiveWave.enemies.Count == 0)
            {
                Debug.LogWarning($"Wave {waveIndex + 1} is empty! Skipping to next wave.");
                waveIndex++;
                continue; // Skip to the next wave
            }
            
            Debug.Log("Starting Wave " + (waveIndex + 1) + " Enemy Scaling: " + currentActiveWave.enemyScaling);
            
            // Notify StatisticsTracker of wave start
            if (StatisticsTracker.Instance != null)
            {
                StatisticsTracker.Instance.OnWaveStart();
            }
            
            // Spawn each enemy in the current wave
            for (int spawnIndex = 0; spawnIndex < currentActiveWave.enemies.Count; spawnIndex++)
            {
                EnemySpawn spawn = currentActiveWave.enemies[spawnIndex];
                
                // Spawn the specified count of this enemy type
                for (int i = 0; i < spawn.count; i++)
                {
                    // Wait first
                    if (i > 0 || spawnIndex > 0)
                    {
                        yield return new WaitForSeconds(spawn.spawnDelay);
                    }

                    Debug.Log("Wave " + (waveIndex + 1) + " - " + 
                              (spawn.enemyType == EnemyType.Delay ? "Delayed by " + spawn.spawnDelay + " Seconds" : 
                               "Spawning " + spawn.enemyType + " (" + (i + 1) + "/" + spawn.count + ")"));
                              
                    SpawnEnemy(spawn.enemyType, spawnPoint.position, spawnPoint.rotation, spawnIndex);
                }
            }

            // Wait until all the children are dead
            yield return new WaitUntil(() => enemyParent.childCount == 0);

            // Notify StatisticsTracker of wave end
            if (StatisticsTracker.Instance != null)
            {
                StatisticsTracker.Instance.OnWaveEnd();
            }

            // Lets do unspeakable things to other systems now that the wave is over
            int waveNumber = waveIndex + 1;
            int incomeRaise = CalculateIncomeRaise(waveNumber);
            bank.IncreasePlayerMoney(currentActiveWave.waveReward + incomeRaise);
            Debug.Log("Wave " + (waveIndex + 1) + " completed!" + " Money earned: " + currentActiveWave.waveReward + " + " + incomeRaise + " raise");
            cleanup.ForceCleanup();

            if (enableWaveBonus && (waveIndex + 1) % waveBonusInterval == 0)
            {
                bank.IncreasePlayerMoney(waveBonusAmount);
                Debug.Log($"Wave bonus! Completed {waveBonusInterval} waves! Bonus reward: {waveBonusAmount}");
            }
            
            // If this was an infinite wave, increment the scaling for next infinite wave            
            if (waveIndex >= waves.Count)
            {
                infiniteWavesCompleted++;
                // Increase the scaling multiplier for the next infinite wave
                currentScalingMultiplier = Mathf.Min(
                    1f + (infiniteWavesCompleted * infiniteWaveScalingIncrement), 
                    maxScalingMultiplier
                );
                Debug.Log($"Infinite wave {infiniteWavesCompleted} completed. Next scaling multiplier: {currentScalingMultiplier:F2}");
            }
            
            // Increment wave index for next loop
            waveIndex++;
            UpdateWaveText(waveIndex + 1);
        }
    }

    // Call this to trigger the next wave
    public void TriggerNextWave()
    {
        nextWaveTriggered = true;
        UpdateWaveText(helperCurrentWave);
    }

    // Separated enemy spawning method
    private void SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation, int spawnIndex)
    {
        if (enemyType != EnemyType.Delay)
        {
            GameObject enemyToSpawn = GetEnemyPrefab(enemyType);
            GameObject newEnemy = Instantiate(enemyToSpawn, position, rotation);
            newEnemy.transform.SetParent(enemyParent);
            
            float enemyScaling = currentActiveWave.enemyScaling;
            
            int shieldCharges = (int)(currentActiveWave.enemies[spawnIndex].shieldCharges * currentScalingMultiplier);
            
            newEnemy.GetComponent<EnemyScript>().Initialize(waypointsParent, playerHealthSystem, enemyScaling, shieldCharges);
        }
    }
}