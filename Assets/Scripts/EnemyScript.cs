using UnityEngine;
using UnityEngine.Rendering;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth = 100; // public for debugging
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int damageToPlayer = 1;

    [Header("References")]
    [SerializeField] private Transform spriteTransform; // Reference to child sprite

    private EnemyPathing pathing;
    private Vector3 previousPosition;

    void Start()
    {
        pathing = GetComponent<EnemyPathing>();
        pathing.Initialize(moveSpeed);
        currentHealth = maxHealth;
        previousPosition = transform.position;
    }
        
    void Update()
    {
        RotateSprite();

        // Check if enemy has reached the end
        if (pathing.HasReachedEnd)
        {
            // Placeholder for applying damage to player
            Debug.Log($"Enemy reached the end! Player takes {damageToPlayer} damage.");
            
            Destroy(gameObject);
        }
    }

    // Rotate sprite based on movement direction
    private void RotateSprite()
    {
        Vector3 moveDirection = (transform.position - previousPosition).normalized;
        if (moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg; //hyi vittu
            spriteTransform.rotation = Quaternion.Euler(0, 0, angle -90); //-90 if facing up by default
        }
        previousPosition = transform.position;
    }

    // Call this method with damage argument when enemy takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // Handle enemy death
    private void Die()
    {
        // Placeholder for death effects        
        Debug.Log("Enemy dead");

        Destroy(gameObject);
    }
}