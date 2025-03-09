using UnityEngine;

public class DONTMERGE_TempProjectileSpawner : MonoBehaviour
{
    public float speed = 100f;
    public GameObject projectilePrefab;
    public float shootInterval = 1f;
    float timer;

    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * speed);
        timer += Time.deltaTime;
        if (timer > shootInterval)
        {
            SpawnProjectile();
            timer = 0f;
        }
        
        
    }

    void SpawnProjectile()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("Projectile prefab not assigned!");
        }
    }
}
