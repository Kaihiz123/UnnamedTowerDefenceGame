using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    [SerializeField]
    private int playerHealth;
    [SerializeField]
    private int maxPlayerHealth;

    public TMPro.TextMeshProUGUI healthText;

    public GameOverScript GameOverScript;

    private void Start()
    {
        playerHealth = PlayerPrefs.GetInt(ISettings.Type.STARTHEALTH.ToString(), 100);
        maxPlayerHealth = PlayerPrefs.GetInt(ISettings.Type.MAXHEALTH.ToString(), 100);
        SettingsManager.Instance.playerHealth = playerHealth; // So far only for PlayerBaseHealthBar
        SettingsManager.Instance.playerMaxHealth = maxPlayerHealth; // So far only for PlayerBaseHealthBar
        UpdatePlayerHealthText();
    }

    private void UpdatePlayerHealthText()
    {
        healthText.text = "" + playerHealth + "/" + maxPlayerHealth;
    }

    public void PlayerTookDamage(int damage)
    {
        playerHealth -= damage;
        SettingsManager.Instance.playerHealth = playerHealth; // So far only for PlayerBaseHealthBar
        UpdatePlayerHealthText();
        if (playerHealth <= 0)
        {
            // Show game over screen
            GameOverScript.ShowGameOverScreen();
        }
    }

    public void IncreasePlayerHealth(int healthIncrease)
    {
        playerHealth += healthIncrease;
        if(playerHealth > maxPlayerHealth)
        {
            playerHealth = maxPlayerHealth;
        }
        SettingsManager.Instance.playerHealth = playerHealth; // So far only for PlayerBaseHealthBar
        UpdatePlayerHealthText();
    }

    public void IncreasePlayerMaxHealth(int healthIncrease)
    {
        maxPlayerHealth += healthIncrease;
        SettingsManager.Instance.playerMaxHealth = maxPlayerHealth; // So far only for PlayerBaseHealthBar
        UpdatePlayerHealthText();
    }
}
