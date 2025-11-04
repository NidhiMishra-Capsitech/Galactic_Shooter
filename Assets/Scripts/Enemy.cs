using UnityEngine;

public enum EnemyType { Normal, TimeAdd, TimeSubtract }

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public EnemyType type = EnemyType.Normal;
    public float speed = 1.5f; 
    public int scoreValue = 100;
    private float originalSpeed; // For slow power-up

    [Header("Time Settings")]
    public float timeToAdd = 5f;
    public float timeToSubtract = 3f;
    
    [Header("Effects")]
    public GameObject explosionVFX; 
    public AudioClip explosionSFX; 

    [HideInInspector]
    public Vector2 moveDirection = Vector2.down;
    
    private AudioSource audioSource;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalSpeed = speed; // Save the original speed
        rb.linearVelocity = moveDirection.normalized * speed;
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    
    public void TakeHit()
    {
        GameManager.Instance.AddScore(scoreValue);

        if (type == EnemyType.TimeAdd)
        {
            GameManager.Instance.AdjustTime(timeToAdd);
        }
        else if (type == EnemyType.TimeSubtract)
        {
            GameManager.Instance.AdjustTime(-timeToSubtract);
        }
        
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }
        
        if (explosionSFX != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, Camera.main.transform.position);
        }
        
        Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // This is called by GameManager to slow/unslow the enemy
    public void SetSlowed(bool slow)
    {
        if (slow)
        {
            speed = originalSpeed * 0.5f;
        }
        else
        {
            speed = originalSpeed;
        }
        // Re-apply the velocity with the new speed
        if (rb != null)
        {
            rb.linearVelocity = moveDirection.normalized * speed;
        }
    }
}