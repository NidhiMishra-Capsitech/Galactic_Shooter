using UnityEngine;

public enum EnemyType { Normal, TimeAdd, TimeSubtract }

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyType type = EnemyType.Normal;
    public float speed = 1.5f; // We slowed this down
    public int scoreValue = 100;

    [Header("Time Settings")]
    public float timeToAdd = 5f;
    public float timeToSubtract = 3f;
    
    [Header("Effects")]
    public GameObject explosionVFX; // The particle effect prefab
    public AudioClip explosionSFX; // The explosion sound file

    private AudioSource audioSource;

    void Start()
    {
        // Give the enemy its initial downward speed
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * speed;
        
        // Add an AudioSource component by code
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    
    public void TakeHit()
    {
        // Tell the GameManager to add score
        GameManager.Instance.AddScore(scoreValue);

        // --- Check what type of enemy this is ---
        if (type == EnemyType.TimeAdd)
        {
            GameManager.Instance.AdjustTime(timeToAdd);
        }
        else if (type == EnemyType.TimeSubtract)
        {
            GameManager.Instance.AdjustTime(-timeToSubtract);
        }
        
        // --- Play Effects ---
        // Play VFX
        if (explosionVFX != null)
        {
            // Spawn the explosion at the enemy's position
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        
        // Play SFX
        if (explosionSFX != null)
        {
            // Play the sound at the camera's position
            AudioSource.PlayClipAtPoint(explosionSFX, Camera.main.transform.position);
        }
        
        // This enemy has been hit, so destroy it
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}