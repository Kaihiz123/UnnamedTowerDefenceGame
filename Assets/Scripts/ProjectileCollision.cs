using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    private Projectile parentProjectile;

    void Start()
    {
        // Find the parent Projectile script
        parentProjectile = GetComponentInParent<Projectile>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is an "Enemy"
        if (other.gameObject.CompareTag("Enemy") && parentProjectile != null)
        {
            EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(parentProjectile.projectileAttackDamage); // Apply damage to enemy
            }

            // Destroy the parent projectile on impact
            Destroy(parentProjectile.gameObject);
        }
    }
}
