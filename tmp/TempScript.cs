using System.Collections.Generic;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    public List<EnemySpawn> enemySpawns;

    public void Spawning()
    {
        // Mitä olitkaan suunnitellu spawnaamiselle ja sille delaylle
    }
}

[System.Serializable]
public class EnemySpawn
{
    public GameObject enemy;
    public float spawnDelay;
}