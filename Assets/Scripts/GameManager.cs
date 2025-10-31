using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    
    // THIS IS THE FIX. It MUST start as false.
    public bool isGameActive = false; 
    
    public float startDelay = 3.5f; // 3 second countdown

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI modeNameText;
    public TextMeshProUGUI livesText; 
    public TextMeshProUGUI countdownText;

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        // We set isGameActive to FALSE here again, just to be safe.
        isGameActive = false; 

        currentMode = DataManager.Instance.selectedMode;
        InitializeGameMode();
    }

    void InitializeGameMode()
    {
        currentScore = 0;
        countdownText.gameObject.SetActive(true); 
        
        if (currentMode == GameMode.Timed)
        {
            gameTimer = timedModeDuration;
            modeNameText.text = "Timed Mode";
            livesText.gameObject.SetActive(false);
        }
        else // Survival Mode
        {
            gameTimer = 0f; 
            lives = 3;      
            modeNameText.text = "Survival Mode";
            livesText.gameObject.SetActive(true); 
            livesText.text = "Lives: " + lives;
        }
        
        scoreText.text = "Score: " + currentScore;
    }

    void Update()
    {
        // --- Countdown Logic ---
        if (startDelay > 0.5f)
        {
            startDelay -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(startDelay - 0.5f).ToString();
            return; // Don't run the rest of Update
        }
        // This runs ONCE when the countdown finishes
        else if (isGameActive == false) 
        {
            isGameActive = true; // THIS is what the spawner is waiting for
            countdownText.text = "GO!";
            Invoke("HideCountdownText", 1f);
        }
        // --- End of Countdown Logic ---

        // The rest of the game loop only runs AFTER isGameActive is true
        if (!isGameActive) return; 

        // --- Main Game Loop ---
        if (currentMode == GameMode.Timed)
        {
            gameTimer -= Time.deltaTime;
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
    
    // ... (Rest of the script: AddScore, AdjustTime, PlayerHit, etc.) ...
    // Make sure your PlayerHit function is the new one that accepts (EnemyType type)
    
    public void PlayerHit(EnemyType type)
    {
        if (!isGameActive) return; 
        
        if (currentMode == GameMode.Survival)
        {
            lives--;
            livesText.text = "Lives: " + lives;
            if (lives <= 0)
            {
                EndGame("Game Over!");
            }
        }
        else // Timed Mode
        {
            if (type == EnemyType.Normal)
            {
                EndGame("Game Over!");
            }
            // If hit by TimeAdd or TimeSubtract, we do nothing
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
        isGameActive = false;
        
        int coinsEarned = currentScore / scorePerCoin;
        DataManager.Instance.AddCoins(coinsEarned);
        DataManager.Instance.SubmitNewScore(currentMode, currentScore);

        PlayerPrefs.SetInt("LastScore", currentScore);
        PlayerPrefs.SetInt("LastCoins", coinsEarned);
        PlayerPrefs.SetString("EndMessage", endMessage);
        
        SceneManager.LoadScene("End");
    }

    // --- (You need these functions, make sure they are in your script) ---
    
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