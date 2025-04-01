using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button nextWaveButton;
    [SerializeField] private bool nextWaveTriggered = false; // visible for testing
    [SerializeField] private int helperCurrentWave = 0;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private int startFromWave = 1;
    [Header("Spawn Settings")] 
    
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

        StartCoroutine(Spawning());
    }

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
                accumulatedMoney += waves[i].waveReward;
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
                
                accumulatedMoney += repeatingWavesSkipped * globalInfiniteWaveReward;
                
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
                yield return new WaitUntil(() => nextWaveTriggered);
                nextWaveButton.gameObject.SetActive(false);
                nextWaveTriggered = false;
            }
            else
            {
                // No buten for repeating waves
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
                
                // Select a random wave from repeating waves
                int randomIndex = Random.Range(0, repeatingWaves.Count);
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
                
                Debug.Log($"Selected repeating wave pattern {randomIndex + 1} with scaling multiplier {currentScalingMultiplier:F2} and reward {modifiedWave.waveReward}");
            }
            
            helperCurrentWave = waveIndex + 1; // update current wave for inspector
            
            // Check if the wave is empty
            if (currentActiveWave.enemies == null || currentActiveWave.enemies.Count == 0)
            {
                Debug.LogWarning($"Wave {waveIndex + 1} is empty! Skipping to next wave.");
                waveIndex++;
                continue; // Skip to the next wave
            }
            
            Debug.Log("Starting Wave " + (waveIndex + 1) + " Enemy Scaling: " + currentActiveWave.enemyScaling);
            
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
                              
                    SpawnEnemy(spawn.enemyType, spawnPoint.position, spawnPoint.rotation);
                }
            }

            // Wait until all the children are dead
            yield return new WaitUntil(() => enemyParent.childCount == 0);

            // Lets do unspeakable things to other systems now that the wave is over
            bank.IncreasePlayerMoney(currentActiveWave.waveReward);
            Debug.Log("Wave " + (waveIndex + 1) + " completed!" + " Money earned: " + currentActiveWave.waveReward);
            
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
        }
    }

    // Call this to trigger the next wave
    public void TriggerNextWave()
    {
        nextWaveTriggered = true;
    }

    // Separated enemy spawning method
    private void SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        if (enemyType != EnemyType.Delay)
        {
            GameObject enemyToSpawn = GetEnemyPrefab(enemyType);
            GameObject newEnemy = Instantiate(enemyToSpawn, position, rotation);
            newEnemy.transform.SetParent(enemyParent);
            
            float enemyScaling = currentActiveWave.enemyScaling;
            
            // Find the current spawn configuration to get shield charges
            int shieldCharges = 0;
            foreach (EnemySpawn spawn in currentActiveWave.enemies)
            {
                if (spawn.enemyType == enemyType)
                {
                    shieldCharges = (int)(spawn.shieldCharges*currentScalingMultiplier);
                    break;
                }
            }
            
            newEnemy.GetComponent<EnemyScript>().Initialize(waypointsParent, playerHealthSystem, enemyScaling, shieldCharges);
        }
    }
}