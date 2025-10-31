using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;

    [Header("Screen Boundaries")]
    private Vector2 minBounds;
    private Vector2 maxBounds;

    [Header("Shooting")]
    public GameObject projectilePrefab; // We will make this next
    public Transform firePoint;
    public float fireRate = 0.15f;  // 4 shots per second
    private float nextFireTime = 0f;

   [Header("Audio")]
    public AudioClip laserSound; // The sound file
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Calculate screen boundaries based on the camera
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // --- Movement Input ---
        // This uses a "drag" control. Wherever you touch/click, the ship goes.
        if (GameManager.Instance.isGameActive == false)
        {
            rb.linearVelocity = Vector2.zero; // Stop the ship
            return; // Skip the rest
        }

        if (Input.touchCount > 0)
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            moveInput = touchPosition;
        }
        else if (Input.GetMouseButton(0)) // For testing in the editor
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            moveInput = mousePosition;
        }

        // --- Shooting Input ---
        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    // FixedUpdate is used for physics
   void FixedUpdate()
{
    // Check if the player is touching or clicking
    if (Input.touchCount > 0 || Input.GetMouseButton(0))
    {
        // Move the ship
        Vector2 targetPosition = Vector2.MoveTowards(rb.position, moveInput, moveSpeed * Time.fixedDeltaTime);

        // Clamp (limit) the position to stay within the screen
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(targetPosition);
    }
}
    void Fire()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            if (laserSound != null && audioSource != null)
            {
                // Play the sound once
                audioSource.PlayOneShot(laserSound);
            }
        }
    }

    // This runs when our trigger collider hits another trigger collider
    void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Enemy"))
    {
        // Get the enemy's script
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Tell the GameManager WHAT hit us
            GameManager.Instance.PlayerHit(enemy.type);
        }
        else
        {
            // If it's an enemy with no script, use a default
            GameManager.Instance.PlayerHit(EnemyType.Normal);
        }
        
        // Destroy the enemy
        Destroy(other.gameObject);
    }
}
}