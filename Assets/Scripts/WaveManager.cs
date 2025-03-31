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
        int startWaveIndex = debugMode ? Mathf.Clamp(startFromWave -1, 0, waves.Count - 1) : 0;

        // Calculate bank balance for skipped waves if debug mode is enabled
        if (debugMode && startWaveIndex > 0)
        {
            int accumulatedMoney = 0;
            for (int i = 0; i < startWaveIndex; i++)
            {
                accumulatedMoney += waves[i].waveReward;
            }
            
            // Add the accumulated money to the player's bank
            if (accumulatedMoney > 0)
            {
                bank.IncreasePlayerMoney(accumulatedMoney);
                Debug.Log($"Debug mode: Added {accumulatedMoney} money for skipped waves (1-{startWaveIndex})");
            }
        }
        
        // Loop through each wave
        for (int waveIndex = startWaveIndex; waveIndex < waves.Count; waveIndex++)
        {
            // Wait for player to trigger wave
            nextWaveButton.gameObject.SetActive(true); // change depending on UI needs
            yield return new WaitUntil(() => nextWaveTriggered);
            nextWaveButton.gameObject.SetActive(false); // change depending on UI needs
            nextWaveTriggered = false;
            
            helperCurrentWave = waveIndex + 1; // update current wave for inspector
            Wave currentWave = waves[waveIndex];
            Debug.Log("Starting Wave " + (waveIndex + 1) + " Enemy Scaling: " + currentWave.enemyScaling);
            
            // Spawn each enemy in the current wave
            for (int spawnIndex = 0; spawnIndex < currentWave.enemies.Count; spawnIndex++)
            {
                EnemySpawn spawn = currentWave.enemies[spawnIndex];
                
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
            bank.IncreasePlayerMoney(currentWave.waveReward);
            Debug.Log("Wave " + (waveIndex + 1) + " completed!" + " Money earned: " + currentWave.waveReward);
        }
        
        Debug.Log("All waves completed!");
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
            
            // Get the current wave and find the enemy spawn configuration
            Wave currentWave = waves[helperCurrentWave - 1];
            float enemyScaling = currentWave.enemyScaling;
            
            // Find the current spawn configuration to get shield charges
            int shieldCharges = 0;
            foreach (EnemySpawn spawn in currentWave.enemies)
            {
                if (spawn.enemyType == enemyType)
                {
                    shieldCharges = spawn.shieldCharges;
                    break;
                }
            }
            
            // Pass shield charges to the Initialize method
            newEnemy.GetComponent<EnemyScript>().Initialize(waypointsParent, playerHealthSystem, enemyScaling, shieldCharges);
        }
    }
}