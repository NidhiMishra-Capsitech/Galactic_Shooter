using UnityEngine;

// NOTICE THE CLASS NAME MATCHES THE FILE NAME
public class PlayerLaser : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 3f; // How long it lives before being destroyed

    void Start()
    {
        // Give the laser its initial speed
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
        
        // Destroy the laser after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit is an enemy
        if (other.CompareTag("Enemy"))
        {
            // Get the Enemy script from the object we hit
            Enemy enemy = other.GetComponent<Enemy>();
            
            // If we found the script, tell it to take a hit
            if (enemy != null)
            {
                enemy.TakeHit();
            }
            
            // The laser is destroyed, but the enemy handles its own destruction
            Destroy(gameObject);
        }
    }
}