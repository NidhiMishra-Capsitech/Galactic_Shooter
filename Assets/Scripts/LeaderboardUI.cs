using UnityEngine;
using TMPro; // Needed for text
using System.Text; // Needed for StringBuilder

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timedScoresText;
    public TextMeshProUGUI survivalScoresText;

    // This function runs automatically every time the panel is ENABLED
    void OnEnable()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        // --- Update Timed Scores ---
        
        // Use a StringBuilder to build our score list. It's very efficient!
        StringBuilder timedBuilder = new StringBuilder();
        
        // Loop through the high scores from our DataManager
        for (int i = 0; i < DataManager.Instance.timedHighScores.Count; i++)
        {
            // Add a line, e.g., "1. 5000"
            timedBuilder.AppendLine($"{(i + 1)}. {DataManager.Instance.timedHighScores[i]}");
        }
        
        // Set the text
        timedScoresText.text = timedBuilder.ToString();

        
        // --- Update Survival Scores ---
        StringBuilder survivalBuilder = new StringBuilder();
        
        for (int i = 0; i < DataManager.Instance.survivalHighScores.Count; i++)
        {
            // Add a line, e.g., "1. 12000"
            survivalBuilder.AppendLine($"{(i + 1)}. {DataManager.Instance.survivalHighScores[i]}");
        }
        
        // Set the text
        survivalScoresText.text = survivalBuilder.ToString();
    }
}