using UnityEngine;
using System.Collections.Generic;

public class TowerShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // The object that gets fired
    public float projectileAttackDamage; // Damage per shot
    public float projectileSpeed; // Speed of the projectile
    public Transform towerTurret;

    public float towerEnemyDetectAreaSize; // Detection radius
    public Transform towerProjectileSpawnLocation;
    private Transform enemyDetectArea;
    private Transform debug_EnemyDetectAreaVisual;
    [SerializeField] private GameObject theDebug_EnemyDetectAreaVisual;
    private CircleCollider2D enemyDetectCollider;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Projectiles per second")]
    public float towerFireRate; // Shots per second
    private float shootInterval; // Time between shots
    private float shootTimer = 0f; // Timer to track shooting
    public AudioClip soundShoot;
    AudioSource audioSource;

    private TowerShootingAoE towerShootingAoEScript;

    void Start()
    {
        towerShootingAoEScript = GetComponent<TowerShootingAoE>();
        audioSource = GetComponentInChildren<AudioSource>();
        shootInterval = 1f / towerFireRate; // Calculate time between shots

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
            // Find the closest enemy from the list
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

            // Rotate to look at the enemy (for 2D, using transform.up)
            towerTurret.transform.up = closestEnemy.transform.position - transform.position;

            // Check if enough time has passed to shoot
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                SpawnProjectile();
                audioSource.PlayOneShot(soundShoot);
                shootTimer = 0f;
            }
        }
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
