using UnityEngine;
using UnityEngine.Rendering;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth = 100; // public for debugging
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int damageToPlayer = 1;

    private EnemyPathing pathing;

    public void Initialize(GameObject waypointsParent)
    {
        pathing = GetComponent<EnemyPathing>();
        pathing.Initialize(moveSpeed, waypointsParent);
        currentHealth = maxHealth;
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