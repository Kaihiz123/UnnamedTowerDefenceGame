using UnityEngine;
using UnityEngine.Rendering;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth = 100; // public for debugging
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int damageToPlayer = 1;
    public GameObject enemySprite;
    public GameObject enemySpriteHit;
    public GameObject explosionPrefab;
    private float hitTimer = 0f;
    public float hitTime;
    public AudioClip soundExplosion;
    AudioSource audioSource;

    private EnemyPathing pathing;

    public void Initialize(GameObject waypointsParent)
    {
        pathing = GetComponent<EnemyPathing>();
        pathing.Initialize(moveSpeed, waypointsParent);
        currentHealth = maxHealth;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check if enemy has reached the end
        if (pathing.HasReachedEnd)
        {
            // Placeholder for applying damage to player
            Debug.Log($"Enemy reached the end! Player takes {damageToPlayer} damage.");
            
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
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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
        // Disable all components except AudioSource
        foreach (Component comp in GetComponentsInChildren<Component>())
        {
            if (!(comp is Transform) && !(comp is AudioSource))
                ((Behaviour)comp).enabled = false;
        }

        // Play explosion sound
        audioSource.PlayOneShot(soundExplosion);

        // Instantiate explosion effect
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        Debug.Log("Enemy dead");

        // Destroy enemy after sound plays
        Destroy(gameObject, soundExplosion.length);
    }


}