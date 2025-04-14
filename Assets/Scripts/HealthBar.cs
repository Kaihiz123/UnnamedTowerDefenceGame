using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform healthBarScaler;
    public SpriteRenderer healthBarColor;
    public EnemyScript enemyScript;
    
    void Update()
    {
        healthBarScaler.transform.localScale = new Vector2(enemyScript.currentHealth / enemyScript.maxHealth, 1f);
        if (healthBarScaler.transform.localScale.x < 0.66f && healthBarScaler.transform.localScale.x > 0.33f)
        {
            healthBarColor.color = Color.yellow;
        }
        else if (healthBarScaler.transform.localScale.x < 0.33f)
        {
            healthBarColor.color = Color.red;
        }
    }
}
