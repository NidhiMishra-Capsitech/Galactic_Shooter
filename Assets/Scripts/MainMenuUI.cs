using UnityEngine;
using UnityEngine.UI; // We must add this to control Buttons

// This script should be on your MainMenuPanel
public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button storeButton;
    public Button leaderboardButton;
    public Button aboutButton;
    public Button exitButton;

    [Header("Panel Back Buttons")]
    public Button modeSelect_BackButton;
    public Button store_BackButton;
    public Button leaderboard_BackButton;
    public Button about_BackButton;
    
    [Header("Mode Select Buttons")] // <-- NEW SECTION
    public Button timedModeButton;
    public Button survivalModeButton;
    
    
    // Start runs AFTER Awake and AFTER OnSceneLoaded (because of our execution order)
    void Start()
    {
        // Find the one, true DataManager that lives forever
        DataManager dm = DataManager.Instance;
        if (dm == null) return; // Safety check

        // --- Wire Main Menu Buttons ---
        if (playButton) { playButton.onClick.RemoveAllListeners(); playButton.onClick.AddListener(dm.ShowModePanel); }
        if (storeButton) { storeButton.onClick.RemoveAllListeners(); storeButton.onClick.AddListener(dm.ShowStorePanel); }
        if (leaderboardButton) { leaderboardButton.onClick.RemoveAllListeners(); leaderboardButton.onClick.AddListener(dm.ShowLeaderboardPanel); }
        if (aboutButton) { aboutButton.onClick.RemoveAllListeners(); aboutButton.onClick.AddListener(dm.ShowAboutPanel); }
        if (exitButton) { exitButton.onClick.RemoveAllListeners(); exitButton.onClick.AddListener(dm.QuitGame); }

        // --- Wire Panel Back Buttons ---
        if (modeSelect_BackButton) { modeSelect_BackButton.onClick.RemoveAllListeners(); modeSelect_BackButton.onClick.AddListener(dm.HideModePanel); }
        if (store_BackButton) { store_BackButton.onClick.RemoveAllListeners(); store_BackButton.onClick.AddListener(dm.HideStorePanel); }
        if (leaderboard_BackButton) { leaderboard_BackButton.onClick.RemoveAllListeners(); leaderboard_BackButton.onClick.AddListener(dm.HideLeaderboardPanel); }
        if (about_BackButton) { about_BackButton.onClick.RemoveAllListeners(); about_BackButton.onClick.AddListener(dm.HideAboutPanel); }
        
        // --- NEWLY ADDED ---
        if (timedModeButton) { timedModeButton.onClick.RemoveAllListeners(); timedModeButton.onClick.AddListener(dm.SelectTimedMode); }
        if (survivalModeButton) { survivalModeButton.onClick.RemoveAllListeners(); survivalModeButton.onClick.AddListener(dm.SelectSurvivalMode); }
    }
}