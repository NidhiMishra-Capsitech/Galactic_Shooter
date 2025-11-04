using UnityEngine;

// This makes a new "Create" menu option in Unity
// We'll use it to create our store items
public enum PowerupType { None, SlowTime, IncreasePower, Explosives, DecreaseSpeed }
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

    [Header("Spaceship Stats")]
    // If this is a ship, what does it use?
    public Sprite shipSprite;
    public GameObject projectilePrefab; // The bullet it fires
    public float fireRate;
    public Vector2 shipScale = new Vector2(1, 1);
    public Vector2 projectileScale = new Vector2(1, 1);
    public Vector2 firePointPosition = new Vector2(0, 1);  
    public Vector2 thrusterPosition = new Vector2(0, -1);

    [Header("Power-up Stats")]
    // This is only used if 'isPowerup' is TRUE
    public PowerupType powerupType = PowerupType.None;
}