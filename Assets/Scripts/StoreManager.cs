using UnityEngine;
using TMPro; // We need this
using System.Collections.Generic; // We need this for Lists

public class StoreManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI coinBalanceText;
    public GameObject spaceshipPanel;
    public GameObject powerupPanel;
    public GameObject notEnoughCoinsPanel; // <-- The new variable

    [Header("Store Items")]
    public List<StoreItemData> allStoreItems; // Our list of all items
    public GameObject storeItemPrefab; // Our "StoreItemCard" prefab

    void Start()
    {
        UpdateCoinBalance();
        PopulateStore();
        
        // Hide the pop-up panel at the start
        if (notEnoughCoinsPanel != null)
        {
            notEnoughCoinsPanel.SetActive(false);
        }
        
        ShowSpaceships(); // Default to showing spaceships
    }

    public void UpdateCoinBalance()
    {
        if (DataManager.Instance != null)
        {
            coinBalanceText.text = "Coin Balance: " + DataManager.Instance.totalCoins;
        }
    }

    // This function builds the store
    void PopulateStore()
    {
        // Clear any old items
        foreach (Transform child in spaceshipPanel.transform) { Destroy(child.gameObject); }
        foreach (Transform child in powerupPanel.transform) { Destroy(child.gameObject); }

        // Loop through all our ScriptableObject items
        foreach (StoreItemData item in allStoreItems)
        {
            // Create a new card
            GameObject itemCardObject = Instantiate(storeItemPrefab);

            // Sort it into the correct panel
            if (item.isPowerup)
            {
                itemCardObject.transform.SetParent(powerupPanel.transform, false);
            }
            else
            {
                itemCardObject.transform.SetParent(spaceshipPanel.transform, false);
            }

            // Get the card's script and set it up
            StoreItemCard cardScript = itemCardObject.GetComponent<StoreItemCard>();
            cardScript.Setup(item);
        }
    }

    // --- Button Functions ---

    public void ShowSpaceships()
    {
        spaceshipPanel.SetActive(true);
        powerupPanel.SetActive(false);
    }

    public void ShowPowerups()
    {
        spaceshipPanel.SetActive(false);
        powerupPanel.SetActive(true);
    }

    // This will be called by our "OK" button on the pop-up
    public void CloseNotEnoughCoinsPanel()
    {
        notEnoughCoinsPanel.SetActive(false);
    }
}