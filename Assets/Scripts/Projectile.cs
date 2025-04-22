using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public float projectileSpeed;
    [HideInInspector]
    public float projectileAttackDamage;
    [HideInInspector]
    public float projectileAoEAttackRangeRadius;
    public float projectileLifetime; // Time before self-destruction
    
    [HideInInspector]
    public TowerInfo.TowerType sourceTowerType; // Tower type that shot this projectile

    void Start()
    {
        // Destroy the projectile after 'lifetime' seconds if it hasn't hit anything
        Destroy(gameObject, projectileLifetime);
    }

    void FixedUpdate()
    {
        // Move forward
        transform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);
    }
}
