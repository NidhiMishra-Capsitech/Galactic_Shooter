using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreItemCard : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI priceText;
    public Button getButton;
    public TextMeshProUGUI getButtonText; 

    [HideInInspector]
    public StoreItemData currentItemData; 
    private StoreManager storeManager; 

    public void Setup(StoreItemData data, StoreManager manager)
    {
        currentItemData = data;
        storeManager = manager;

        itemIcon.sprite = data.itemIcon;
        
        // --- THIS IS THE FIX ---
        // We now check against the "Item Name" field, e.g., "Default Ship"
        if (DataManager.Instance.equippedShipID == currentItemData.itemName)
        {
            getButton.interactable = false;
            priceText.text = "Owned";
            getButtonText.text = "Equipped";
        }
        else if (DataManager.Instance.IsShipUnlocked(currentItemData.itemName))
        {
            getButton.interactable = true;
            priceText.text = "Owned";
            getButtonText.text = "Select";
        }
        else
        {
            getButton.interactable = true;
            priceText.text = data.price + " Coins";
            getButtonText.text = "Buy";
        }
        
        getButton.onClick.RemoveAllListeners();
        getButton.onClick.AddListener(OnGetButtonClicked);
    }

   public void OnGetButtonClicked()
    {
        // --- NEW LOGIC ---
        if (currentItemData.isPowerup)
        {
            // --- This is the Power-up Buy Logic ---
            if (DataManager.Instance.totalCoins >= currentItemData.price)
            {
                // 1. Subtract coins
                DataManager.Instance.AddCoins(-currentItemData.price);
                // 2. Add the power-up
                DataManager.Instance.AddPowerup(currentItemData.powerupType, 1);
                // 3. Refresh coin display
                storeManager.UpdateCoinBalance();
                // 4. (Optional) Show a "Purchased!" pop-up
                Debug.Log("Bought 1 " + currentItemData.itemName);
            }
            else
            {
                // Not enough coins
                storeManager.notEnoughCoinsPanel.SetActive(true);
            }
        }
        else
        {
            // --- This is the Spaceship Buy/Select Logic ---
            if (DataManager.Instance.IsShipUnlocked(currentItemData.itemName))
            {
                DataManager.Instance.EquipShip(currentItemData.itemName);
            }
            else
            {
                if (DataManager.Instance.totalCoins >= currentItemData.price)
                {
                    DataManager.Instance.AddCoins(-currentItemData.price);
                    DataManager.Instance.UnlockShip(currentItemData.itemName);
                    DataManager.Instance.EquipShip(currentItemData.itemName);
                }
                else
                {
                    storeManager.notEnoughCoinsPanel.SetActive(true);
                }
            }
            
            // Refresh all cards to show new "Equipped" state
            storeManager.RefreshAllCards();
        }
    }
}