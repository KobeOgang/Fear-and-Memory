using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Instance;
    public static bool IsInspecting = false;

    [Header("UI Elements")]
    public GameObject inspectionUI;      // Parent canvas for name, description UI
    public TMP_Text itemNameText;        // TextMeshPro for item name
    public TMP_Text itemDescriptionText; // TextMeshPro for item description

    [Header("Inspection Settings")]
    public Transform inspectionTransform; // Reference for item placement

    [Header("Document Reading UI")]
    public GameObject inspectionFullTextPanel;  // Drag the panel you created
    public TMP_Text inspectionFullTextDisplay;  // Drag the text element from inside the panel
    public TMP_Text readTextPrompt;


    private GameObject currentItem;   // Currently inspected item
    private bool isInspecting = false;

    private GameObject originalItem;
    private bool isReadingDocument = false;

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

    private void Start()
    {
        if (inspectionUI != null)
        {
            inspectionUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isInspecting)
        {
            if (currentItem.GetComponent<InteractableItem>().itemData.itemType == ItemData.ItemType.Document)
            {
                // If we press Space, toggle the full text view
                if (Input.GetKeyDown(KeyCode.F))
                {
                    isReadingDocument = !isReadingDocument;
                    inspectionFullTextPanel.SetActive(isReadingDocument);

                    // Optional: Hide the 3D item model while reading to prevent clutter
                    currentItem.SetActive(!isReadingDocument);
                }
            }

            // If we are reading, don't process mouse clicks for taking/leaving
            if (isReadingDocument) return;

            // Rotate the item based on mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            currentItem.transform.Rotate(Vector3.up, -mouseX * 5f, Space.World);
            currentItem.transform.Rotate(Vector3.right, mouseY * 5f, Space.World);

            // Add item to inventory and exit inspection mode
            if (Input.GetMouseButtonDown(0))
            {
                // Get the item data from the inspected item
                ItemData dataOfInspectedItem = currentItem.GetComponent<InteractableItem>().itemData;

                if (dataOfInspectedItem.itemType == ItemData.ItemType.Document)
                {
                    // It's a document, add it to the Codex
                    CodexManager.Instance.AddDocument(dataOfInspectedItem);

                    // Since it's added to the codex, we can destroy the original and end inspection
                    Destroy(originalItem);
                    Destroy(currentItem);
                    EndInspection();
                }
                else
                {
                    // Attempt to add item to inventory
                    bool added = InventoryManager.Instance.AddItem(currentItem.GetComponent<InteractableItem>().itemData);
                    if (added)
                    {
                        Destroy(originalItem); // Destroy the original item in the scene
                        Destroy(currentItem);  // Destroy the inspected item's 3D object
                        EndInspection();      // End inspection mode
                    }
                    else
                    {
                        Debug.LogWarning("Item could not be added to inventory (likely inventory is full).");
                    }
                }
   
            }

            // Exit inspection mode without adding the item
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(currentItem);
                EndInspection();
            }
        }

    }

    public void StartInspection(ItemData itemData, GameObject itemObject)
    {

        IsInspecting = true;

        // Store reference to the original item
        originalItem = itemObject;

        // Pause the game and lock camera
        Time.timeScale = 0;
        /*Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;*/

        // Enable UI
        inspectionUI.SetActive(true);
        itemNameText.text = itemData.itemName;
        itemDescriptionText.text = itemData.description;

        if (itemData.itemType == ItemData.ItemType.Document)
        {
            readTextPrompt.gameObject.SetActive(true);
            inspectionFullTextDisplay.text = itemData.itemText; // Pre-load the text
        }

        // Place cloned item for inspection
        currentItem = Instantiate(itemObject, inspectionTransform.position, inspectionTransform.rotation);
        currentItem.GetComponent<Collider>().enabled = false; // Disable its collider
        currentItem.transform.SetParent(inspectionTransform);

        isInspecting = true;
    }


    public void EndInspection()
    {
        isReadingDocument = false;
        if (inspectionFullTextPanel != null) inspectionFullTextPanel.SetActive(false);
        if (readTextPrompt != null) readTextPrompt.gameObject.SetActive(false);

        IsInspecting = false;
        // Resume the game and unlock camera
        Time.timeScale = 1;
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/

        // Disable UI
        inspectionUI.SetActive(false);

        // Remove inspected item
        Destroy(currentItem);
        currentItem = null;

        isInspecting = false;
    }

}
