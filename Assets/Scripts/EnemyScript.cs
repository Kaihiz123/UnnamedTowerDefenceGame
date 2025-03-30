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

    private EnemyPathing pathing;
    public AudioClip soundHit;

    PlayerHealthSystem playerHealthSystem;

    HealthBar healthBar;

    public void Initialize(GameObject waypointsParent, PlayerHealthSystem playerHealthSystem, float enemyScaling)
    {
        pathing = GetComponent<EnemyPathing>();
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.gameObject.SetActive(PlayerPrefs.GetInt(ISettings.Type.SHOWENEMYHEALTHBAR.ToString(), 1) == 1);
        pathing.Initialize(moveSpeed, waypointsParent);
        maxHealth *= enemyScaling;
        currentHealth = maxHealth;
        this.playerHealthSystem = playerHealthSystem;
    }

    void Update()
    {
        // Check if enemy has reached the end
        if (pathing.HasReachedEnd)
        {
            // Placeholder for applying damage to player
            Debug.Log($"Enemy reached the end! Player takes {damageToPlayer} damage.");

            playerHealthSystem.PlayerTookDamage(damageToPlayer);

            Destroy(gameObject);
        }

        hitTimer += Time.deltaTime;

        if (hitTimer >= hitTime)
        {
            enemySprite.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            enemySpriteHit.SetActive(false);
        }
    }


    // Call this method with damage argument when enemy takes damage
    public void TakeDamage(float damage)
    {
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

        Debug.Log("Enemy dead");

        // Destroy enemy after sound plays
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