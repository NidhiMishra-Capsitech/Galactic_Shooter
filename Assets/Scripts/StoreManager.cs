using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StoreManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI coinBalanceText;
    public GameObject spaceshipPanel;
    public GameObject powerupPanel;
    public GameObject notEnoughCoinsPanel;

    [Header("Store Items")]
    public List<StoreItemData> allStoreItems;
    public GameObject storeItemPrefab;

    private List<StoreItemCard> activeCards = new List<StoreItemCard>();

    void Start()
    {
        UpdateCoinBalance();
        PopulateStore();
        if (notEnoughCoinsPanel != null) notEnoughCoinsPanel.SetActive(false);
        ShowSpaceships(); 
    }
    
    void OnEnable()
    {
        UpdateCoinBalance();
        RefreshAllCards();
    }

    public void UpdateCoinBalance()
    {
        if (DataManager.Instance != null)
            coinBalanceText.text = "Coin Balance: " + DataManager.Instance.totalCoins;
    }

    void PopulateStore()
    {
        activeCards.Clear(); 
        foreach (Transform child in spaceshipPanel.transform) { Destroy(child.gameObject); }
        foreach (Transform child in powerupPanel.transform) { Destroy(child.gameObject); }

        foreach (StoreItemData item in allStoreItems)
        {
            GameObject itemCardObject = Instantiate(storeItemPrefab);
            
            if (item.isPowerup)
                itemCardObject.transform.SetParent(powerupPanel.transform, false);
            else
                itemCardObject.transform.SetParent(spaceshipPanel.transform, false);

            StoreItemCard cardScript = itemCardObject.GetComponent<StoreItemCard>();
            cardScript.Setup(item, this);
            activeCards.Add(cardScript); 
        }
    }

    public void RefreshAllCards()
    {
        UpdateCoinBalance();
        foreach (StoreItemCard card in activeCards)
        {
            // --- THIS IS THE FIX ---
            // The old code was buggy. This is simpler and correct.
            if (card != null && card.currentItemData != null)
            {
                card.Setup(card.currentItemData, this); 
            }
        }
    }

    // --- Button Functions ---
    public void ShowSpaceships() { if (spaceshipPanel != null) spaceshipPanel.SetActive(true); if (powerupPanel != null) powerupPanel.SetActive(false); }
    public void ShowPowerups() { if (spaceshipPanel != null) spaceshipPanel.SetActive(false); if (powerupPanel != null) powerupPanel.SetActive(true); }
    public void CloseNotEnoughCoinsPanel() { if (notEnoughCoinsPanel != null) notEnoughCoinsPanel.SetActive(false); }
}