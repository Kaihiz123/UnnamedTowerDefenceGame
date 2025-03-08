using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Settings")] 
    [SerializeField] private Transform spawnPoint;

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
    }

    // Represents a complete wave of enemies
    [System.Serializable]
    public class Wave
    {
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
        StartCoroutine(Spawning());
    }

    // Main spawning coroutine that handles wave progression
    public IEnumerator Spawning()
    {
        // Loop through each wave
        for (int waveIndex = 0; waveIndex < waves.Count; waveIndex++)
        {
            Debug.Log("Starting Wave " + (waveIndex + 1));
            Wave currentWave = waves[waveIndex];
            
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
            // Wait for a short duration before starting the next wave (debug/testing purposes)
            Debug.Log("Wave " + (waveIndex + 1) + " completed! Waiting 5 seconds before next wave.");
            yield return new WaitForSeconds(5f);
        }
        
        Debug.Log("All waves completed!");
    }

    // Separated enemy spawning method
    private void SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        if (enemyType != EnemyType.Delay)
        {
            GameObject enemyToSpawn = GetEnemyPrefab(enemyType);
            Instantiate(enemyToSpawn, position, rotation);
        }
    }

}