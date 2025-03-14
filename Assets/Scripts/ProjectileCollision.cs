using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    private Projectile parentProjectile;
    public GameObject explosionPrefab;
    public GameObject aoEEffectPrefab;

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

        // For AoE, make it always explode with particles on hit
        if (other.gameObject.CompareTag("Enemy") && parentProjectile != null && gameObject.CompareTag("ProjectileAoE"))
        {
            // Instantiate explosion effect
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, transform.rotation);
            GameObject aoEEffectInstance = Instantiate(aoEEffectPrefab, transform.position, transform.rotation);
            explosionInstance.transform.localScale = new Vector3(2f,2f,2f);
            AoEEffect aoEEffectScript = aoEEffectInstance.GetComponent<AoEEffect>();
            if (aoEEffectScript != null)
            {
                aoEEffectScript.aoEAttackDamage = parentProjectile.projectileAttackDamage;
            }
        }
    }
}
