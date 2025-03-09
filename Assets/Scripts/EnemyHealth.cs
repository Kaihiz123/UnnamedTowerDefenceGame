using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100; // Enemy starting health

    public void TakeDamage(int damage)
    {
        health -= damage; // Reduce health
        Debug.Log(gameObject.name + " took " + damage + " damage! Remaining health: " + health);

        if (health <= 0)
        {
            Die(); // Call Die() if health is 0 or below
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject); // Destroy enemy object
    }
}
