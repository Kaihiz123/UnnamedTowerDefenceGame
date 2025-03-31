using UnityEngine;

public class AoEEffect : MonoBehaviour
{
    [HideInInspector]
    public float aoEAttackDamage;
    public CircleCollider2D aoECollider;
    [HideInInspector]
    public float aoEAttackRangeRadius;
    public GameObject explosionPrefab;
    public Transform DEBUGAoEAttackRangeCircleSprite;
    [SerializeField] private GameObject theDEBUGAoEAttackRangeCircleSprite;

    void Start()
    { 
        aoECollider.radius = aoEAttackRangeRadius;
        DEBUGAoEAttackRangeCircleSprite.localScale = new Vector3(aoEAttackRangeRadius * 0.02f, aoEAttackRangeRadius * 0.02f, 1f);
        Destroy(gameObject, Time.deltaTime);  
    }

    void Update()
    {
        if (SettingsManager.Instance.DebugON == true)
        {
            theDEBUGAoEAttackRangeCircleSprite.SetActive(true);
        }
        else
        {
            theDEBUGAoEAttackRangeCircleSprite.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the "Enemy" tag
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                // Check if the enemy has a shield
                if (enemy.shieldCharges > 0)
                {
                    // Skip this enemy, shield blocks AoE damage
                    return;
                }
                
                // No shield, apply damage normally
                enemy.TakeDamage(aoEAttackDamage); 

                // Instantiate explosionPrefab at enemy's position, with no parent
                GameObject explosion = Instantiate(explosionPrefab, enemy.transform.position, Quaternion.identity);
                explosion.transform.SetParent(null); // Ensure it's unparented (at scene root)
            }
        }
    }

}
