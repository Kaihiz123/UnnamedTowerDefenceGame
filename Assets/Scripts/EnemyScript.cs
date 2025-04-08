using UnityEngine;
using UnityEngine.Rendering;

public class EnemyScript : MonoBehaviour
{

    [Header("Enemy Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100; // public for debugging
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int damageToPlayer = 1;
    public GameObject enemySprite;
    public GameObject enemySpriteHit;
    public GameObject explosionPrefab;
    private float hitTimer = 0f;
    public float hitTime;

    [Header("Shield Settings")]


    public int shieldCharges = 0;
    [SerializeField] private GameObject shieldVisualPrefab;
    private GameObject activeShieldVisual; // Reference to the instantiated shield
    private bool hasShield => shieldCharges > 0;

    private EnemyPathing pathing;
    public AudioClip soundHit;

    public AudioClip shieldHitSound;

    PlayerHealthSystem playerHealthSystem;
    HealthBar healthBar;
    

    public void Initialize(GameObject waypointsParent, PlayerHealthSystem playerHealthSystem, float enemyScaling, int shieldCharges = 0)
    {
        pathing = GetComponent<EnemyPathing>();
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.gameObject.SetActive(PlayerPrefs.GetInt(ISettings.Type.SHOWENEMYHEALTHBAR.ToString(), 1) == 1);
        pathing.Initialize(moveSpeed, waypointsParent);
        maxHealth *= enemyScaling;
        currentHealth = maxHealth;
        this.playerHealthSystem = playerHealthSystem;
        
        // Set shield charges
        this.shieldCharges = shieldCharges;
        
        // Create shield visual if this enemy has shield charges
        UpdateShieldVisual();
    }

    void Update()
    {
        // Check if enemy has reached the end
        if (pathing.HasReachedEnd)
        {
            // Placeholder for applying damage to player
            Debug.Log($"Enemy reached the end! Player takes {damageToPlayer} damage.");

            playerHealthSystem.PlayerTookDamage(damageToPlayer);
            
            Die();

        }

        hitTimer += Time.deltaTime;

        if (hitTimer >= hitTime)
        {
            enemySprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            enemySpriteHit.SetActive(false);
        }
    }

    // Handle shield hits
    public bool HitShield()
    {
        if (hasShield)
        {
            shieldCharges--;
            UpdateShieldVisual();
            
            // Play shield hit sound
            if (shieldHitSound != null)
                AudioManager.Instance.PlaySoundEffect(shieldHitSound);
                
            return true; // Shield absorbed the hit
        }
        return false; // No shield to absorb hit
    }

    private void UpdateShieldVisual()
    {

        // If we have shield charges but no active shield visual, create one
        if (hasShield && activeShieldVisual == null && shieldVisualPrefab != null)
        {

            activeShieldVisual = Instantiate(shieldVisualPrefab, transform.position, Quaternion.identity);
            activeShieldVisual.transform.SetParent(transform); // Parent to the enemy
            activeShieldVisual.transform.localPosition = Vector3.zero; // Center on the enemy
        }
        // If we have no shield charges but still have an active shield visual, destroy it
        else if (!hasShield && activeShieldVisual != null)
        {
            Destroy(activeShieldVisual);
            activeShieldVisual = null;
        }
    }

    // Call this method with damage argument when enemy takes damage
    public void TakeDamage(float damage, TowerInfo.TowerType sourceTowerType = TowerInfo.TowerType.Basic)
    {
        // Calculate effective damage and overkill
        float effectiveDamage = Mathf.Min(damage, currentHealth);
        float overkillDamage = damage > currentHealth ? damage - currentHealth : 0;
        
        // Record effective and overkill damage
        if (StatisticsTracker.Instance != null)
        {
            StatisticsTracker.Instance.RecordEffectiveDamage(sourceTowerType, effectiveDamage);
        
            if (overkillDamage > 0)
            {
                StatisticsTracker.Instance.RecordOverkillDamage(sourceTowerType, overkillDamage);
            }
        }
        
        // Apply the damage
        currentHealth -= damage;
        AudioManager.Instance.PlaySoundEffect(soundHit);

        enemySprite.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
        enemySpriteHit.SetActive(true);
        hitTimer = 0f;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // Handle enemy death
    private void Die()
    {
        // Instantiate explosion effect
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        Debug.Log("Enemy [" + gameObject.name + "] dead");

        // Destroy enemy after sound plays (mikä ääni?)
        Destroy(gameObject);
    }

    private void ShowHealthBar(bool show)
    {
        healthBar.gameObject.SetActive(show);
    }

    private void OnEnable()
    {
        GameManager.OnShowEnemyHealthBar += ShowHealthBar;
    }

    private void OnDisable()
    {
        GameManager.OnShowEnemyHealthBar -= ShowHealthBar;
    }
}