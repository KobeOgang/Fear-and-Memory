using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject inventoryUIParent; // The entire inventory UI panel

    [Header("List Elements")]
    public Transform itemListContentParent;   // The parent object for the vertical list
    public GameObject itemListItemPrefab;     // The prefab for each item in the list

    [Header("Display Elements")]
    public Image displayItemIcon;
    public TMP_Text displayItemName;
    public TMP_Text displayItemDescription;

    void Start()
    {
        // Start with the inventory hidden
        inventoryUIParent.SetActive(false);
    }

    void Update()
    {
        // Toggle inventory with Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // If another menu is already open, AND we are trying to open this one, do nothing.
            if (GameUIManager.isMenuOpen && !inventoryUIParent.activeSelf)
            {
                return;
            }

            bool isActive = !inventoryUIParent.activeSelf;
            inventoryUIParent.SetActive(isActive);

            // Update the global state flag
            GameUIManager.isMenuOpen = isActive;

            if (isActive)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                PopulateItemList();
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void PopulateItemList()
    {
        // Clear old list items before repopulating
        foreach (Transform child in itemListContentParent)
        {
            Destroy(child.gameObject);
        }

        // Exit if inventory is empty
        if (InventoryManager.Instance.inventory.Count == 0)
        {
            // Clear the display panel if no items are left
            displayItemIcon.gameObject.SetActive(false);
            displayItemName.text = "";
            displayItemDescription.text = "Inventory is empty.";
            return;
        }

        // Create a new list item for each item in the inventory
        for (int i = InventoryManager.Instance.inventory.Count - 1; i >= 0; i--)
        {
            ItemData item = InventoryManager.Instance.inventory[i]; // Get the item at the current index

            GameObject listItem = Instantiate(itemListItemPrefab, itemListContentParent);
            listItem.GetComponentInChildren<TMP_Text>().text = item.itemName;
            listItem.GetComponent<Button>().onClick.AddListener(() => DisplayItem(item));
        }

        // Automatically select and display the first item in the list
        DisplayItem(InventoryManager.Instance.inventory[InventoryManager.Instance.inventory.Count - 1]);
    }

    public void DisplayItem(ItemData data)
    {
        displayItemName.text = data.itemName;
        displayItemDescription.text = data.description;

        if (data.icon != null)
        {
            displayItemIcon.sprite = data.icon;
            displayItemIcon.gameObject.SetActive(true);
        }
        else
        {
            displayItemIcon.gameObject.SetActive(false);
        }
    }
}
