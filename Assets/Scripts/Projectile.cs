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

    void Start()
    {
        // Destroy the projectile after 'lifetime' seconds if it hasn't hit anything
        Destroy(gameObject, projectileLifetime);
    }

    void Update()
    {
        // Move forward
        transform.Translate(Vector3.up * projectileSpeed * Time.deltaTime);
    }
}
