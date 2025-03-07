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
        [InspectorName("Tank Enemy")] TankEnemy
    }

    // Represents a single enemy spawn configuration
    [System.Serializable]
    public class EnemySpawn
    {
        public EnemyType enemyType;
        // Delay before spawning the next enemy in seconds
        public float spawnDelay;
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
                GameObject enemyToSpawn = GetEnemyPrefab(spawn.enemyType);
                
                Debug.Log("Wave " + (waveIndex + 1) + " - Spawning " + spawn.enemyType);
                Instantiate(enemyToSpawn, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(spawn.spawnDelay);
            }

            // Wait for a short duration before starting the next wave
            Debug.Log("Wave " + (waveIndex + 1) + " completed! Waiting 5 seconds before next wave.");
            yield return new WaitForSeconds(5f);
        }
        
        Debug.Log("All waves completed!");
    }
}