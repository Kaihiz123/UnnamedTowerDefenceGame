using UnityEngine;

public class AoEEffect : MonoBehaviour
{
    [HideInInspector]
    public int aoEAttackDamage;
    public GameObject explosionPrefab;

    void Awake()
    {
        // Destroy the parent projectile on impact after [float] seconds
        Destroy(gameObject, 1.0f);   
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided objects are with tag "Enemy"
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(aoEAttackDamage); // Apply damage to enemy
                Instantiate(explosionPrefab, enemy.transform);
            }
        }
    }
}
