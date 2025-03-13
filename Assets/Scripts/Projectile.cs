using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public int projectileSpeed;
    [HideInInspector]
    public int projectileAttackDamage;
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
