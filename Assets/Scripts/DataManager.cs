
using UnityEngine;
using UnityEngine.SceneManagement; // Needed to change scenes
using System.Collections.Generic; // Needed for Lists (for leaderboards)

// We list our two possible game modes here.
public enum GameMode { Timed, Survival }

public class DataManager : MonoBehaviour
{
    // --- Singleton Instance ---
    // This makes it so we can access this script from anywhere
    public static DataManager Instance;

    [Header("UI Panels")]
    // Drag your panel GameObjects from the Hierarchy here
    public GameObject mainMenuPanel;
    public GameObject modeSelectPanel;
    public GameObject storePanel;
    public GameObject leaderboardPanel;
    public GameObject aboutPanel;

    [Header("Game Data")]
    public GameMode selectedMode; // Remembers which mode we picked
    public int totalCoins;         // Remembers our total coins

    [Header("Leaderboards")]
    public List<int> timedHighScores = new List<int>();
    public List<int> survivalHighScores = new List<int>();
    const int leaderboardLength = 5; // We will only save the top 5

    
    void Awake()
    {
        // --- This is the Singleton pattern ---
        if (Instance == null)
        {
            Instance = this;
            // Tell Unity: "Do NOT destroy this object when loading a new scene."
            DontDestroyOnLoad(gameObject);
            
            // Load all saved data from the phone
            LoadData(); 
        }
        else
        {
            // If one *already* exists, destroy this new, duplicate one.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Make sure all panels are hidden when the game starts,
        // except the main menu panel.
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
        if (storePanel != null) storePanel.SetActive(false);
        if (leaderboardPanel != null) leaderboardPanel.SetActive(false);
        if (aboutPanel != null) aboutPanel.SetActive(false);
    }

    // --- Panel Control Functions (Called by Buttons) ---

    public void ShowModePanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        modeSelectPanel.SetActive(true);
    }
    public void HideModePanel()
    {
        modeSelectPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowStorePanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        storePanel.SetActive(true);
    }
    public void HideStorePanel()
    {
        storePanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowLeaderboardPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        // We'll add a call here later to update the text
        // FindObjectOfType<LeaderboardUI>().UpdateDisplay();
    }
    public void HideLeaderboardPanel()
    {
        leaderboardPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowAboutPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        aboutPanel.SetActive(true);
    }
    public void HideAboutPanel()
    {
        aboutPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // --- Scene & Game Functions (Called by Buttons) ---

    // Call this from your "Timed Mode" button
    public void SelectTimedMode()
    {
        selectedMode = GameMode.Timed;
        SceneManager.LoadScene("Game"); // "Game" must match your scene file name
    }

    // Call this from your "Survival Mode" button
    public void SelectSurvivalMode()
    {
        selectedMode = GameMode.Survival;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game..."); // This shows in the Unity console
        Application.Quit();
    }

    // --- Data & Leaderboard Functions ---

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveData(); // Save right away
    }

    // This is called by GameManager when a game ends
    public void SubmitNewScore(GameMode mode, int score)
    {
        List<int> scoreList = (mode == GameMode.Timed) ? timedHighScores : survivalHighScores;
        
        scoreList.Add(score);
        scoreList.Sort((a, b) => b.CompareTo(a)); // Sort high-to-low
        
        // Keep only the top 5
        if (scoreList.Count > leaderboardLength)
        {
            scoreList.RemoveRange(leaderboardLength, scoreList.Count - leaderboardLength);
        }
        
        SaveLeaderboards();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        // Load total coins, default to 0 if not found
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        LoadLeaderboards();
    }

    private void SaveLeaderboards()
    {
        for(int i = 0; i < leaderboardLength; i++)
        {
            // Make sure we have a score to save, otherwise save 0
            int timedScore = (i < timedHighScores.Count) ? timedHighScores[i] : 0;
            int survivalScore = (i < survivalHighScores.Count) ? survivalHighScores[i] : 0;
            
            PlayerPrefs.SetInt("TimedScore_" + i, timedScore);
            // This line is now fixed
            PlayerPrefs.SetInt("SurvivalScore_" + i, survivalScore);
        }
    }

    private void LoadLeaderboards()
    {
        timedHighScores.Clear();
        survivalHighScores.Clear();
        
        for(int i = 0; i < leaderboardLength; i++)
        {
            timedHighScores.Add(PlayerPrefs.GetInt("TimedScore_" + i, 0));
            survivalHighScores.Add(PlayerPrefs.GetInt("SurvivalScore_" + i, 0));
        }
    }
}