using UnityEngine;

public class DangerZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // First, check if the object is an enemy
        if (other.CompareTag("Enemy"))
        {
            // Get the enemy's script
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // IF a "Normal" enemy gets past, end the game
                if (enemy.type == EnemyType.Normal)
                {
                    // Tell the GameManager to end the game
                    GameManager.Instance.EndGame("An enemy got past!");
                }
            }
            
            // Destroy the enemy (whether it's normal or not)
            Destroy(other.gameObject);
        }
    }
}