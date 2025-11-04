using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameMode { Timed, Survival }

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject modeSelectPanel;
    public GameObject storePanel;
    public GameObject leaderboardPanel;
    public GameObject aboutPanel;

    [Header("Game Data")]
    public GameMode selectedMode;
    public int totalCoins;

    [Header("Player Inventory")]
    public string equippedShipID = "Default Ship"; 
    public List<string> unlockedShipIDs = new List<string>();
    
    // Counters for all four power-ups
    public int slowTimePowerups = 0;
    public int increasePowerPowerups = 0;
    public int explosivesPowerups = 0;
    public int decreaseSpeedPowerups = 0;

    [Header("Leaderboards")]
    public List<int> timedHighScores = new List<int>();
    public List<int> survivalHighScores = new List<int>();
    const int leaderboardLength = 5;

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- Scene Loading Fix ---
    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null) return;
            
            Transform panelTransform;
            panelTransform = canvas.transform.Find("MainMenuPanel");
            if (panelTransform) mainMenuPanel = panelTransform.gameObject;

            panelTransform = canvas.transform.Find("ModeSelectPanel");
            if (panelTransform) modeSelectPanel = panelTransform.gameObject;

            panelTransform = canvas.transform.Find("StorePanel");
            if (panelTransform) storePanel = panelTransform.gameObject;
            
            panelTransform = canvas.transform.Find("LeaderboardPanel");
            if (panelTransform) leaderboardPanel = panelTransform.gameObject;

            panelTransform = canvas.transform.Find("AboutPanel");
            if (panelTransform) aboutPanel = panelTransform.gameObject;

            // Set the default state
            if (mainMenuPanel) mainMenuPanel.SetActive(true);
            if (modeSelectPanel) modeSelectPanel.SetActive(false);
            if (storePanel) storePanel.SetActive(false);
            if (leaderboardPanel) leaderboardPanel.SetActive(false);
            if (aboutPanel) aboutPanel.SetActive(false);
        }
    }

    // --- Panel Control Functions ---
    public void ShowModePanel() { if (mainMenuPanel) mainMenuPanel.SetActive(false); if (modeSelectPanel) modeSelectPanel.SetActive(true); }
    public void HideModePanel() { if (modeSelectPanel) modeSelectPanel.SetActive(false); if (mainMenuPanel) mainMenuPanel.SetActive(true); }
    public void ShowStorePanel() { if (mainMenuPanel) mainMenuPanel.SetActive(false); if (storePanel) storePanel.SetActive(true); }
    public void HideStorePanel() { if (storePanel) storePanel.SetActive(false); if (mainMenuPanel) mainMenuPanel.SetActive(true); }
    public void ShowLeaderboardPanel() { if (mainMenuPanel) mainMenuPanel.SetActive(false); if (leaderboardPanel) leaderboardPanel.SetActive(true); }
    public void HideLeaderboardPanel() { if (leaderboardPanel) leaderboardPanel.SetActive(false); if (mainMenuPanel) mainMenuPanel.SetActive(true); }
    public void ShowAboutPanel() { if (mainMenuPanel) mainMenuPanel.SetActive(false); if (aboutPanel) aboutPanel.SetActive(true); }
    public void HideAboutPanel() { if (aboutPanel) aboutPanel.SetActive(false); if (mainMenuPanel) mainMenuPanel.SetActive(true); }

    // --- Scene & Game Functions ---
    public void SelectTimedMode() { selectedMode = GameMode.Timed; SceneManager.LoadScene("Game"); }
    public void SelectSurvivalMode() { selectedMode = GameMode.Survival; SceneManager.LoadScene("Game"); }
    public void QuitGame() { Debug.Log("Quitting Game..."); Application.Quit(); }

    // --- NEW INVENTORY FUNCTIONS ---
    public void UnlockShip(string shipItemName)
    {
        if (!unlockedShipIDs.Contains(shipItemName))
        {
            unlockedShipIDs.Add(shipItemName);
            SaveData();
        }
    }

    public void EquipShip(string shipItemName)
    {
        equippedShipID = shipItemName;
        SaveData();
    }

    public bool IsShipUnlocked(string shipItemName)
    {
        return unlockedShipIDs.Contains(shipItemName);
    }
    
    // This function handles all power-up types
    public void AddPowerup(PowerupType type, int amount)
    {
        if (type == PowerupType.SlowTime)
            slowTimePowerups += amount;
        else if (type == PowerupType.IncreasePower)
            increasePowerPowerups += amount;
        else if (type == PowerupType.Explosives)
            explosivesPowerups += amount;
        else if (type == PowerupType.DecreaseSpeed)
            decreaseSpeedPowerups += amount;

        SaveData(); // Save the new amount
    }
    
    // --- Data & Leaderboard Functions ---
    public void AddCoins(int amount) { totalCoins += amount; SaveData(); }
    public void SubmitNewScore(GameMode mode, int score) 
    {
        List<int> scoreList = (mode == GameMode.Timed) ? timedHighScores : survivalHighScores;
        scoreList.Add(score);
        scoreList.Sort((a, b) => b.CompareTo(a)); 
        if (scoreList.Count > leaderboardLength)
        {
            scoreList.RemoveRange(leaderboardLength, scoreList.Count - leaderboardLength);
        }
        SaveLeaderboards();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.SetString("EquippedShipID", equippedShipID);
        
        string unlocked = string.Join(",", unlockedShipIDs);
        PlayerPrefs.SetString("UnlockedShipIDs", unlocked);
        
        // Save all power-up counts
        PlayerPrefs.SetInt("SlowTimePowerups", slowTimePowerups);
        PlayerPrefs.SetInt("IncreasePowerPowerups", increasePowerPowerups);
        PlayerPrefs.SetInt("ExplosivesPowerups", explosivesPowerups);
        PlayerPrefs.SetInt("DecreaseSpeedPowerups", decreaseSpeedPowerups);

        PlayerPrefs.Save();
        // ... save leaderboard data ...
    }

    public void LoadData()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        equippedShipID = PlayerPrefs.GetString("EquippedShipID", "Default Ship");
        
        string[] unlockedArray = PlayerPrefs.GetString("UnlockedShipIDs", "Default Ship").Split(',');
        unlockedShipIDs = new List<string>(unlockedArray);

        // Load all power-up counts
        slowTimePowerups = PlayerPrefs.GetInt("SlowTimePowerups", 0);
        increasePowerPowerups = PlayerPrefs.GetInt("IncreasePowerPowerups", 0);
        explosivesPowerups = PlayerPrefs.GetInt("ExplosivesPowerups", 0);
        decreaseSpeedPowerups = PlayerPrefs.GetInt("DecreaseSpeedPowerups", 0);

        LoadLeaderboards();
    }
    
    private void SaveLeaderboards() 
    {
        for(int i = 0; i < leaderboardLength; i++)
        {
            int timedScore = (i < timedHighScores.Count) ? timedHighScores[i] : 0;
            int survivalScore = (i < survivalHighScores.Count) ? survivalHighScores[i] : 0;
            PlayerPrefs.SetInt("TimedScore_" + i, timedScore);
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