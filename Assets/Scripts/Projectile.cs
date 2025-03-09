using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int projectileSpeed = 5;
    public int attackDamage = 5;
    public float lifetime = 3f; // Time before self-destruction

    void Start()
    {
        // Destroy the projectile after 'lifetime' seconds if it hasn't hit anything
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is an "Enemy"
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Try to get the EnemyHealth component
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage); // Reduce health
            }

            // Destroy the projectile on impact
            Destroy(gameObject);
        }
    }
}
