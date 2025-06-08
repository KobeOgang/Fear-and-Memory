using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CodexUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject codexUIParent; // The entire codex UI canvas/panel
    public GameObject fullTextPanel;   // The panel that shows the full itemText

    [Header("List Elements")]
    public Transform listContentParent; // A vertical layout group to hold the list items
    public GameObject documentListItemPrefab; // A prefab for each item in the list

    [Header("Display Elements")]
    public TMP_Text displayNameText;
    public TMP_Text displayDescriptionText;
    public TMP_Text displayFullText;
    public Image displayIconImage; // REVISED: Using a simple Image for the icon
    public GameObject readTextPrompt;

    private ItemData currentSelectedDocument;

    void Start()
    {
        // Start with the UI hidden
        codexUIParent.SetActive(false);
        fullTextPanel.SetActive(false);
    }

    void Update()
    {
        // Toggle codex with 'J' for Journal (or any key you prefer)
        if (Input.GetKeyDown(KeyCode.J))
        {
            // If another menu is already open, AND we are trying to open this one, do nothing.
            if (GameUIManager.isMenuOpen && !codexUIParent.activeSelf)
            {
                return;
            }

            bool isActive = !codexUIParent.activeSelf;
            codexUIParent.SetActive(isActive);

            // Update the global state flag
            GameUIManager.isMenuOpen = isActive;

            if (isActive)
            {
                // When we open the UI:
                Time.timeScale = 0f; // Pause the game
                Cursor.lockState = CursorLockMode.None; // Unlock the cursor
                Cursor.visible = true; // Make the cursor visible
                PopulateDocumentList();
            }
            else
            {
                // When we close the UI:
                Time.timeScale = 1f; // Resume the game
                Cursor.lockState = CursorLockMode.Locked; // Lock the cursor again
                Cursor.visible = false; // Hide the cursor again
            }
        }

        // Handle showing the full text with Spacebar
        if (codexUIParent.activeSelf && currentSelectedDocument != null)
        {
            // Using GetKey allows the player to hold Space to read
            if (Input.GetKeyDown(KeyCode.F))
            {
                fullTextPanel.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                fullTextPanel.SetActive(false);
            }
        }
    }

    void PopulateDocumentList()
    {
        // Clear old list items
        foreach (Transform child in listContentParent)
        {
            Destroy(child.gameObject);
        }

        // Check if any documents have been collected
        if (CodexManager.Instance.collectedDocuments.Count == 0)
        {
            // If not, hide the display elements and show an empty message
            displayIconImage.gameObject.SetActive(false);
            displayNameText.text = "";
            displayDescriptionText.text = "No documents collected.";

            // Also make sure the "Read Full Text" prompt is hidden
            if (fullTextPanel != null)
                readTextPrompt.SetActive(false);

            return; // Stop the method here
        }

        // This part only runs if documents exist
        for (int i = CodexManager.Instance.collectedDocuments.Count - 1; i >= 0; i--)
        {
            ItemData doc = CodexManager.Instance.collectedDocuments[i]; // Get the document at the current index

            GameObject listItem = Instantiate(documentListItemPrefab, listContentParent);
            listItem.GetComponentInChildren<TMP_Text>().text = doc.itemName;
            listItem.GetComponent<Button>().onClick.AddListener(() => DisplayDocument(doc));
        }

        // Automatically display the first document and show the prompt
        DisplayDocument(CodexManager.Instance.collectedDocuments[CodexManager.Instance.collectedDocuments.Count - 1]);

        if (readTextPrompt != null)
            readTextPrompt.SetActive(true);
    }

    public void DisplayDocument(ItemData data)
    {
        currentSelectedDocument = data;

        displayNameText.text = data.itemName;
        displayDescriptionText.text = data.description;
        displayFullText.text = data.itemText;

        // Display the item's icon
        if (data.icon != null)
        {
            displayIconImage.sprite = data.icon;
            displayIconImage.gameObject.SetActive(true);
        }
        else
        {
            displayIconImage.gameObject.SetActive(false);
        }

        fullTextPanel.SetActive(false);
    }
}
