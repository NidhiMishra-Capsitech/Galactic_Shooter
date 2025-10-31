using UnityEngine;
using UnityEngine.UI; // Needed for UI.Image
using TMPro; // Needed for TextMeshPro

public class StoreItemCard : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI priceText;
    public Button getButton;

    private StoreItemData currentItemData;

    // This function is called by the StoreManager to fill in the card's info
    public void Setup(StoreItemData data)
    {
        currentItemData = data;

        // Set the UI
        itemIcon.sprite = data.itemIcon;
        priceText.text = data.price + " Coins";
        
        // We'll add logic here later to check if it's already "Owned"
        
        // Set the button's click listener
        getButton.onClick.AddListener(OnGetButtonClicked);
    }

    // This is called when the "Get" button is clicked
    public void OnGetButtonClicked()
    {
        Debug.Log("Trying to buy: " + currentItemData.itemName);

        // Check if the player has enough money
        if (DataManager.Instance.totalCoins >= currentItemData.price)
        {
            // --- Purchase Logic ---
            DataManager.Instance.AddCoins(-currentItemData.price); // Subtract coins
            
            // Add code here to "unlock" the item
            // e.g., PlayerPrefs.SetInt("Unlocked_" + currentItemData.itemName, 1);

            Debug.Log("Purchased: " + currentItemData.itemName);
            
            // Disable the button after purchase
            getButton.interactable = false;
            priceText.text = "Owned";
            
            // We need to update the coin balance text in the store
            FindObjectOfType<StoreManager>().UpdateCoinBalance();
        }
        else
        {
            // --- This is the updated part ---
            // Show the "Not Enough Coins" panel
            FindObjectOfType<StoreManager>().notEnoughCoinsPanel.SetActive(true);
        }
    }
}