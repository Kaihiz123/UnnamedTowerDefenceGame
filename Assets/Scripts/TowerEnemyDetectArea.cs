using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectArea : MonoBehaviour
{
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public TowerShooting tower; // Reference to the main tower script

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Add(other.gameObject);
            tower.UpdateEnemies(enemiesInRange);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            tower.UpdateEnemies(enemiesInRange);
        }
    }
}
