using UnityEngine;

public class PlayerBaseHealthBar : MonoBehaviour
{
    public Transform healthBarScaler;
    public SpriteRenderer healthBarColor;

    void Update()
    {
        healthBarScaler.transform.localScale = new Vector2(1f, SettingsManager.Instance.playerHealth / SettingsManager.Instance.playerMaxHealth);
        if (healthBarScaler.transform.localScale.y < 0.66f && healthBarScaler.transform.localScale.y > 0.33f)
        {
            healthBarColor.color = Color.yellow;
        }
        else if (healthBarScaler.transform.localScale.y < 0.33f)
        {
            healthBarColor.color = Color.red;
        }
        if (healthBarScaler.transform.localScale.y <= 0f)
        {
            healthBarScaler.transform.localScale = new Vector2(0f, 0f);
        }
    }
}
