using UnityEngine;
using System.Collections.Generic;

public enum TargetingStrategy
{
    Closest,    // Target the closest enemy
    Strongest,  // Target the enemy with the most health
}

public class TowerShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // The object that gets fired
    public float projectileAttackDamage; // Damage per shot
    public float projectileSpeed; // Speed of the projectile
    public Transform towerTurret;
    
    [Header("Targeting")]
    public TargetingStrategy targetingStrategy = TargetingStrategy.Closest;
    public bool stickyTargeting = false; // When true, keeps targeting the same enemy until it's out of range or dead

    public float towerEnemyDetectAreaSize; // Detection radius
    public Transform towerProjectileSpawnLocation;
    private Transform enemyDetectArea;
    private Transform debug_EnemyDetectAreaVisual;
    [SerializeField] private GameObject theDebug_EnemyDetectAreaVisual;
    private CircleCollider2D enemyDetectCollider;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private GameObject currentTarget; // Track the current target for sticky targeting

    [Header("Projectiles per second")]
    public float towerFireRate; // Shots per second
    public float shootInterval; // Time between shots
    private float shootTimer = 0f; // Timer to track shooting
    public AudioClip soundShoot;

    private TowerShootingAoE towerShootingAoEScript;

    void Start()
    {
        towerShootingAoEScript = GetComponent<TowerShootingAoE>();
        UpdateFireRate();
        UpdateDebugCircleRadius();
    }

    public void UpdateDebugCircleRadius()
    {
        // Find the child object named "EnemyDetectArea"
        enemyDetectArea = transform.Find("TowerEnemyDetectArea");
        if (enemyDetectArea != null)
        {
            // Get the CircleCollider2D from the child object
            enemyDetectCollider = enemyDetectArea.GetComponent<CircleCollider2D>();
            if (enemyDetectCollider != null)
            {
                enemyDetectCollider.radius = towerEnemyDetectAreaSize; // Set detection range
                enemyDetectCollider.isTrigger = true;
            }
        }
        else
        {
            Debug.LogError("EnemyDetectArea child object not found!");
        }
        debug_EnemyDetectAreaVisual = transform.Find("DEBUG_EnemyDetectAreaVisual");
        debug_EnemyDetectAreaVisual.localScale = new Vector2(towerEnemyDetectAreaSize / 50f, towerEnemyDetectAreaSize / 50f);
    }  

    public void UpdateFireRate()
    {
        shootInterval = 1f / towerFireRate; // Calculate time between shots
    }

    void Update()
    {
        if (SettingsManager.Instance.DebugON == true)
        {
            theDebug_EnemyDetectAreaVisual.SetActive(true);
        }
        else
        {
            theDebug_EnemyDetectAreaVisual.SetActive(false);
        }

        if (enemiesInRange.Count > 0)
        {
            // Get target based on targeting strategy
            GameObject targetEnemy = GetTargetEnemy();
            
            if (targetEnemy != null)
            {
                // Rotate to look at the enemy (for 2D, using transform.up)
                towerTurret.transform.up = targetEnemy.transform.position - transform.position;

                // Check if enough time has passed to shoot
                shootTimer += Time.deltaTime;
                if (shootTimer >= shootInterval)
                {
                    SpawnProjectile();
                    AudioManager.Instance.PlaySoundEffect(soundShoot);
                    shootTimer = 0f;
                }
            }
        }
        else
        {
            // Clear current target when no enemies are in range
            currentTarget = null;
        }
    }

    GameObject GetTargetEnemy()
    {   
        if (enemiesInRange.Count == 0)
            return null;
            
        // Check if we have a target and sticky targeting is enabled
        if (stickyTargeting && currentTarget != null)
        {
            // Check if the current target is still valid
            if (enemiesInRange.Contains(currentTarget))
            {
                return currentTarget;
            }
        }
        
        // Else get a new target based on the selected targeting strategy
        GameObject newTarget;
        switch (targetingStrategy)
        {
            case TargetingStrategy.Closest:
                newTarget = GetClosestEnemy();
                break;
            case TargetingStrategy.Strongest:
                newTarget = GetStrongestEnemy();
                break;
            default:
                newTarget = GetClosestEnemy(); // Default to closest
                break;
        }
        
        // Update the current target
        currentTarget = newTarget;
        return newTarget;
    }
    
    GameObject GetClosestEnemy()
    {
        // Old closest enemy code reused as is
        GameObject closestEnemy = enemiesInRange[0];
        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                if (Vector2.Distance(transform.position, enemy.transform.position) < Vector2.Distance(transform.position, closestEnemy.transform.position))
                {
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }

    GameObject GetStrongestEnemy()
    {
        GameObject strongestEnemy = enemiesInRange[0];
        float highestHealth = 0f;
        
        // Get the health of the first enemy
        EnemyScript firstEnemyScript = strongestEnemy.GetComponent<EnemyScript>();
        if (firstEnemyScript != null)
        {
            highestHealth = firstEnemyScript.currentHealth;
        }
        
        // Loop through all enemies to find the one with highest health
        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                if (enemyScript != null && enemyScript.currentHealth > highestHealth)
                {
                    highestHealth = enemyScript.currentHealth;
                    strongestEnemy = enemy;
                }
            }
        }
        return strongestEnemy;
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, towerProjectileSpawnLocation.position, towerTurret.transform.rotation); // Create the projectile
            
            // Assign attack damage if the projectile has a script with an attackDamage variable
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.projectileAttackDamage = projectileAttackDamage;
                projectileScript.projectileSpeed = projectileSpeed;

                if (towerShootingAoEScript != null)
                {
                    projectileScript.projectileAoEAttackRangeRadius = towerShootingAoEScript.projectileAoEAttackRangeRadius;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            Debug.LogWarning("Projectile prefab not assigned!");
        }
    }

    public void UpdateEnemies(List<GameObject> updatedEnemies)
    {
        enemiesInRange = updatedEnemies;
    }
}