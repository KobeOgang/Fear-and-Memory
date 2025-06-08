using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory Settings")]
    public int maxSlots = 15;
    public List<ItemData> inventory = new List<ItemData>();

    // NEW: A reference to the UI script so we can tell it to update.
    public InventoryUI inventoryUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Add this line
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // NOTE: The Update method with the Tab key logic has been REMOVED from this script.

    public bool AddItem(ItemData item)
    {
        if (inventory.Count >= maxSlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        inventory.Add(item);
        Debug.Log("Added item: " + item.itemName);

        // Tell the UI to refresh itself if it's open
        if (inventoryUI != null && inventoryUI.inventoryUIParent.activeSelf)
        {
            inventoryUI.PopulateItemList();
        }

        return true;
    }
}
