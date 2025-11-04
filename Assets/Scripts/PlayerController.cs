using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    [HideInInspector]
    public SpriteRenderer playerSprite;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private Vector2 moveInput;

    [Header("Screen Boundaries")]
    private Vector2 minBounds;
    private Vector2 maxBounds;

    [Header("Shooting")]
    public GameObject projectilePrefab; 
    public Transform firePoint;
    public Transform thrusterVFX;
    public float fireRate = 0.25f; 
    private float nextFireTime = 0f;
    private Vector2 projectileScale = new Vector2(1, 1); // <-- NEW: To store the scale

    [Header("Audio")]
    public AudioClip laserSound;
    public float laserVolume = 0.5f;
    private AudioSource audioSource;
    
    [Header("Game Data")]
    public List<StoreItemData> allItemData;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        playerSprite = GetComponent<SpriteRenderer>(); 
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
        
        LoadEquippedShip();
    }
    
    void LoadEquippedShip()
    {
        string equippedID = DataManager.Instance.equippedShipID;
        Debug.Log("Player is trying to equip ship with ID: " + equippedID);
        
        foreach (StoreItemData data in allItemData)
        {
            if (data.itemName == equippedID)
            {
                Debug.Log("Found matching ship: " + data.itemName);
                
                // Apply all stats
                this.playerSprite.sprite = data.shipSprite;
                this.projectilePrefab = data.projectilePrefab;
                this.fireRate = data.fireRate;
                this.transform.localScale = data.shipScale; 
                this.projectileScale = data.projectileScale; 
                
                this.firePoint.localPosition = data.firePointPosition;
            if (thrusterVFX != null)
            {
                this.thrusterVFX.localPosition = data.thrusterPosition;
            } 

                if(data.shipSprite == null) {
                    Debug.LogError("Error: The Ship Sprite for " + data.itemName + " is EMPTY!");
                }
                return; 
            }
        }
        Debug.LogError("Error: Could not find any ship data for ID: " + equippedID);
    }

    void Update()
    {
        // (Update code is unchanged)
        if (GameManager.Instance.isGameActive == false)
        {
            rb.linearVelocity = Vector2.zero; 
            return; 
        }
        if (Input.touchCount > 0)
        {
            Vector2 touchPosition = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            moveInput = touchPosition;
        }
        else if (Input.GetMouseButton(0)) 
        {
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            moveInput = mousePosition;
        }
        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        // (FixedUpdate code is unchanged)
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            Vector2 targetPosition = Vector2.MoveTowards(rb.position, moveInput, moveSpeed * Time.fixedDeltaTime);
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            rb.MovePosition(targetPosition);
        }
    }

    // --- UPDATED FIRE METHOD ---
    void Fire()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // 1. Create the laser
            GameObject laser = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // 2. Apply the custom scale
            laser.transform.localScale = this.projectileScale;
            
            // 3. Play the sound
            if (laserSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(laserSound, laserVolume);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // (OnTriggerEnter2D code is unchanged)
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                GameManager.Instance.PlayerHit(enemy.type);
            }
            else
            {
                GameManager.Instance.PlayerHit(EnemyType.Normal);
            }
            Destroy(other.gameObject);
        }
    }
}