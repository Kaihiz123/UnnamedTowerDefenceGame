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
        // Destroy the parent projectile on impact after [float] seconds
        Destroy(gameObject, 1.0f);   
        aoECollider.radius = aoEAttackRangeRadius;
        DEBUGAoEAttackRangeCircleSprite.localScale = new Vector3(aoEAttackRangeRadius * 0.02f, aoEAttackRangeRadius * 0.02f, 1f);
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
