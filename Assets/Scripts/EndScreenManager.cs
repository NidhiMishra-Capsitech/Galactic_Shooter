using UnityEngine;
using UnityEngine.SceneManagement; // To load scenes
using TMPro; // To control text

public class EndScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI coinsEarnedText;

    void Start()
    {
        // Get the data that GameManager saved for us
        // PlayerPrefs is a simple way to save data between scenes
        string endMessage = PlayerPrefs.GetString("EndMessage", "Game Over");
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        int lastCoins = PlayerPrefs.GetInt("LastCoins", 0);

        // Set the UI text
        titleText.text = endMessage;
        finalScoreText.text = "Final Score: " + lastScore;
        coinsEarnedText.text = "Coins Earned: " + lastCoins;
    }

    // --- Button Functions ---

    // This will be called by the "Retry" button
    public void OnRetryButton()
    {
        // Load the Game scene again
        // Note: The DataManager will still have the same mode selected
        SceneManager.LoadScene("Game");
    }

    // This will be called by the "Main Menu" button
    public void OnMainMenuButton()
    {
        // Load the MainMenu scene
        SceneManager.LoadScene("MainMenu");
    }
}