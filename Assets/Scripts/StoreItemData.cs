using UnityEngine;

// This makes a new "Create" menu option in Unity
// We'll use it to create our store items
[CreateAssetMenu(fileName = "New Store Item", menuName = "Store/Store Item")]
public class StoreItemData : ScriptableObject
{
    // Data for every item
    public string itemName;
    public Sprite itemIcon;
    public int price;
    public bool isPowerup; // To know which tab it belongs to
    
    // We'll add this later
    // public GameObject itemPrefab; // The actual spaceship prefab to give the player
}