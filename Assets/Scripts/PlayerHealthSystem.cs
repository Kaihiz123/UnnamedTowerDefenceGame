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
        playerHealth = PlayerPrefs.GetInt(ISettings.Type.STARTHEALTH.ToString(), 10);
        maxPlayerHealth = PlayerPrefs.GetInt(ISettings.Type.MAXHEALTH.ToString(), 10);
        UpdatePlayerHealthText();
    }

    private void UpdatePlayerHealthText()
    {
        healthText.text = "" + playerHealth + "/" + maxPlayerHealth;
    }

    public void PlayerTookDamage(int damage)
    {
        playerHealth -= damage;
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
        UpdatePlayerHealthText();
    }

    public void IncreasePlayerMaxHealth(int healthIncrease)
    {
        maxPlayerHealth += healthIncrease;
        UpdatePlayerHealthText();
    }
}
