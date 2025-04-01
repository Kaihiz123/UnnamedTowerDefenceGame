using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    private Projectile parentProjectile;
    public GameObject explosionPrefab;
    public GameObject aoEEffectPrefab;
    [SerializeField] private GameObject sparkEffectPrefab;
    [SerializeField] private GameObject shieldHitEffectPrefab;
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
                // Check for shield first
                bool shieldAbsorbedHit = enemy.HitShield();
                
                if (shieldAbsorbedHit)
                {
                    // Shield absorbed the hit, spawn shield hit effect if available
                    if (shieldHitEffectPrefab != null)
                    {
                        Instantiate(shieldHitEffectPrefab, transform.position, Quaternion.Euler(0, 0, 180) * parentProjectile.transform.rotation);
                    }
                }
                else
                {
                    // No shield - check if it's an AoE projectile
                    bool isAoEProjectile = gameObject.CompareTag("ProjectileAoE");
                    
                    if (!isAoEProjectile)
                    {
                        enemy.TakeDamage(parentProjectile.projectileAttackDamage);
                    }
                    else 
                    {
                        enemy.TakeDamage(0); // still removes shield charges on direct hit
                    }
                    
                    // Hit spark particles
                    if (sparkEffectPrefab != null)
                    {                                                
                        Instantiate(sparkEffectPrefab, transform.position, Quaternion.Euler(0, 0, 180) * parentProjectile.transform.rotation);
                    }
                }
            }

            Destroy(parentProjectile.gameObject);
        }

        // For AoE, make it always explode with particles on hit
        if (other.gameObject.CompareTag("Enemy") && parentProjectile != null && gameObject.CompareTag("ProjectileAoE"))
        {
            // Instantiate explosion effect
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, transform.rotation);
            GameObject aoEEffectInstance = Instantiate(aoEEffectPrefab, transform.position, transform.rotation);            
            /* Explosion explosionScript = explosionInstance.GetComponent<Explosion>();
            if (explosionScript != null && explosionScript.theParticleSystem != null)
            {
                // Modify the Start Lifetime of the Particle System
                var theParticleSystem = explosionScript.theParticleSystem.main;
                theParticleSystem.startLifetime = parentProjectile.projectileAoEAttackRangeRadius / 2f;
            }
            else
            {
                Debug.LogError("Explosion script or Particle System is missing on the instantiated object!");
            } */
            
            AoEEffect aoEEffectScript = aoEEffectInstance.GetComponent<AoEEffect>();
            if (aoEEffectScript != null)
            {
                aoEEffectScript.aoEAttackDamage = parentProjectile.projectileAttackDamage;
                aoEEffectScript.aoEAttackRangeRadius = parentProjectile.projectileAoEAttackRangeRadius; 
            }
        }
    }
}
