using UnityEngine;
using UnityEngine.SceneManagement; // Needed to load scenes
using TMPro; // Needed to control TextMeshPro
using System.Collections; // We need this for Coroutines!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

    [Header("Game Mode Settings")]
    private GameMode currentMode;
    public float timedModeDuration = 45f;
    public int scorePerCoin = 100;

    [Header("Live Game Stats")]
    public int currentScore;
    public float gameTimer; 
    public int lives;       
    public bool isGameActive = false; 
    public float startDelay = 3.5f; 

    [Header("Invincibility")]
    public bool isPlayerInvulnerable = false; 
    public float invincibilityDuration = 3f;  
    public float enemyPushbackAmount = 4f; 
    private PlayerController playerController; 

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI modeNameText;
    public GameObject livesUIParent; 
    public TextMeshProUGUI countdownText;
    public GameObject lifeLostIndicator; 

    // --- NEW POWER-UP SECTION ---
    [Header("Power-up UI")]
    public TextMeshProUGUI slowTimeCountText;
    public TextMeshProUGUI increasePowerCountText;
    public TextMeshProUGUI explosivesCountText;
    public TextMeshProUGUI decreaseSpeedCountText;
    
    [Header("Power-up States")]
    public bool isSlowTimeActive = false;
    public bool isPowerShotActive = false;
    public bool isEnemySlowActive = false;
    public bool isTimerSlowed = false;
    private float playerOriginalFireRate;
    public float explosionRadius = 7f; // How big the bomb is
    public GameObject bombExplosionVFX; // The visual effect for the bomb
// ... (your other variables) // To store the player's normal fire rate

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        isGameActive = false; 
        isPlayerInvulnerable = false; 
        
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
            Debug.LogError("GameManager: Could not find PlayerController!");

        currentMode = DataManager.Instance.selectedMode;
        InitializeGameMode();
        
        UpdatePowerupUI(); // Update counts at start
    }

    void InitializeGameMode()
    {
        currentScore = 0;
        countdownText.gameObject.SetActive(true); 
        lifeLostIndicator.SetActive(false); 
        
        if (currentMode == GameMode.Timed)
        {
            gameTimer = timedModeDuration;
            modeNameText.text = "Timed Mode";
            livesUIParent.SetActive(false); 
        }
        else // Survival Mode
        {
            gameTimer = 0f; 
            lives = 3;      
            modeNameText.text = "Survival Mode";
            livesUIParent.SetActive(true); 
            UpdateLivesText(); 
        }
        
        scoreText.text = "Score: " + currentScore;
    }

    void Update()
    {
        // Countdown Logic
        if (startDelay > 0.5f)
        {
            startDelay -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(startDelay - 0.5f).ToString();
            return; 
        }
        else if (isGameActive == false && !lifeLostIndicator.activeInHierarchy) // Check if life lost text is active
        {
            isGameActive = true; 
            countdownText.text = "GO!";
            Invoke("HideCountdownText", 1f);
        }

        if (!isGameActive) return; 

        // Main Game Loop
        if (currentMode == GameMode.Timed)
        {
             
    float timeToDecrease = Time.deltaTime;
    if (isTimerSlowed)
    {
        timeToDecrease *= 0.5f;  
    }
    gameTimer -= timeToDecrease;
 
            if (gameTimer <= 0)
            {
                gameTimer = 0;
                EndGame("Time's Up!");
            }
            UpdateTimerText(gameTimer);
        }
        else // Survival Mode
        {
            gameTimer += Time.deltaTime; 
            UpdateTimerText(gameTimer);
        }
    }
    
    // --- POWER-UP FUNCTIONS ---

    public void UpdatePowerupUI()
    {
        if (DataManager.Instance == null) return; // Safety check
        slowTimeCountText.text = "x" + DataManager.Instance.slowTimePowerups;
        increasePowerCountText.text = "x" + DataManager.Instance.increasePowerPowerups;
        explosivesCountText.text = "x" + DataManager.Instance.explosivesPowerups;
        decreaseSpeedCountText.text = "x" + DataManager.Instance.decreaseSpeedPowerups;
    }

    // --- 1. SLOW TIME ---
    public void UseSlowTimePowerup()
{
    // Power-up only works in Timed Mode
    if (DataManager.Instance.slowTimePowerups > 0 && !isSlowTimeActive && isGameActive && currentMode == GameMode.Timed)
    {
        DataManager.Instance.AddPowerup(PowerupType.SlowTime, -1); 
        UpdatePowerupUI();
        StartCoroutine(SlowTimeCoroutine());
    }
}

   IEnumerator SlowTimeCoroutine()
{
    isSlowTimeActive = true;
    isTimerSlowed = true; // Tell the Update loop to slow the timer
    yield return new WaitForSeconds(5f); // Effect lasts 5 real-time seconds
    isTimerSlowed = false; // Timer returns to normal
    isSlowTimeActive = false;
}

    // --- 2. EXPLOSIVES ---
    public void UseExplosivesPowerup()
{
    if (DataManager.Instance.explosivesPowerups > 0 && isGameActive)
    {
        DataManager.Instance.AddPowerup(PowerupType.Explosives, -1);
        UpdatePowerupUI();

        // 1. Spawn a big explosion visual at the player's location
        if (bombExplosionVFX != null)
        {
            Instantiate(bombExplosionVFX, playerController.transform.position, Quaternion.identity);
        }

        // 2. Find all colliders within the radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerController.transform.position, explosionRadius);

        // 3. Loop through them and destroy only enemies
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Call TakeHit() so the player gets score and it plays the enemy's own vfx
                    enemy.TakeHit(); 
                }
            }
        }
    }
}

    // --- 3. INCREASE POWER ---
    public void UseIncreasePowerPowerup()
    {
        if (DataManager.Instance.increasePowerPowerups > 0 && !isPowerShotActive && isGameActive)
        {
            DataManager.Instance.AddPowerup(PowerupType.IncreasePower, -1);
            UpdatePowerupUI();
            StartCoroutine(IncreasePowerCoroutine());
        }
    }

    IEnumerator IncreasePowerCoroutine()
    {
        isPowerShotActive = true;
        // Store the player's original rate and then make it faster
        playerOriginalFireRate = playerController.fireRate;
        playerController.fireRate = playerOriginalFireRate * 0.5f; // 50% faster!
        
        yield return new WaitForSeconds(5f); // Effect lasts 5 seconds
        
        // Return to normal
        playerController.fireRate = playerOriginalFireRate;
        isPowerShotActive = false;
    }

    // --- 4. DECREASE ENEMY SPEED ---
    public void UseDecreaseSpeedPowerup()
    {
        if (DataManager.Instance.decreaseSpeedPowerups > 0 && !isEnemySlowActive && isGameActive)
        {
            DataManager.Instance.AddPowerup(PowerupType.DecreaseSpeed, -1);
            UpdatePowerupUI();
            StartCoroutine(DecreaseSpeedCoroutine());
        }
    }

    IEnumerator DecreaseSpeedCoroutine()
    {
        isEnemySlowActive = true;
        // Slow all current enemies
        SlowAllEnemies(true);
        
        yield return new WaitForSeconds(8f); // Effect lasts 8 seconds
        
        isEnemySlowActive = false;
        // Restore speed for all current enemies
        SlowAllEnemies(false);
    }

    // Helper function for the DecreaseSpeed power-up
    void SlowAllEnemies(bool slow)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.GetComponent<Enemy>().SetSlowed(slow);
            }
        }
    }
    
    // --- (Rest of your script: PlayerHit, HandlePlayerHitRecovery, EndGame, etc.) ---
    
    public void PlayerHit(EnemyType type)
    {
        if (isPlayerInvulnerable || !isGameActive) return; 
        
        if (DataManager.Instance.selectedMode == GameMode.Survival)
        {
            StartCoroutine(HandlePlayerHitRecovery());
        }
        else // Timed Mode
        {
            if (type == EnemyType.Normal)
            {
                EndGame("Game Over!");
            }
        }
    }

    IEnumerator HandlePlayerHitRecovery()
    {
        if (lives <= 0)
        {
            yield break; 
        }
        
        lives--;
        UpdateLivesText();
        
        if (lives <= 0)
        {
            EndGame("Game Over!");
            yield break; 
        }
        
        lifeLostIndicator.SetActive(true);
        
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.transform.position += Vector3.up * enemyPushbackAmount;
            }
        }
        
        isPlayerInvulnerable = true;
        float blinkTimer = 0;
        float blinkInterval = 0.1f;
        
        while (blinkTimer < invincibilityDuration)
        {
            if (playerController != null && playerController.playerSprite != null)
            {
                playerController.playerSprite.enabled = !playerController.playerSprite.enabled;
            }
            
            blinkTimer += blinkInterval;
            
            if(blinkTimer > 1.5f)
            {
                lifeLostIndicator.SetActive(false);
            }
            
            yield return new WaitForSeconds(blinkInterval);
        }
        
        if (playerController != null && playerController.playerSprite != null)
        {
            playerController.playerSprite.enabled = true; 
        }
        isPlayerInvulnerable = false; 
        lifeLostIndicator.SetActive(false); 
    }

    void UpdateLivesText()
    {
        TextMeshProUGUI livesText = livesUIParent.GetComponentInChildren<TextMeshProUGUI>();
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }
    
    void HideCountdownText()
    {
        countdownText.gameObject.SetActive(false);
    }

    void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void EndGame(string endMessage)
    {
        if (playerController != null) playerController.playerSprite.enabled = true;
        isGameActive = false;
        
        int coinsEarned = currentScore / scorePerCoin;
        DataManager.Instance.AddCoins(coinsEarned);
        DataManager.Instance.SubmitNewScore(currentMode, currentScore);

        PlayerPrefs.SetInt("LastScore", currentScore);
        PlayerPrefs.SetInt("LastCoins", coinsEarned);
        PlayerPrefs.SetString("EndMessage", endMessage);
        
        SceneManager.LoadScene("End");
    }
    
    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        currentScore += amount;
        scoreText.text = "Score: " + currentScore;
    }

    public void AdjustTime(float amount)
    {
        if (currentMode == GameMode.Timed)
        {
            gameTimer += amount;
        }
    }
}