using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;         // Reference to the ScriptableObject data
    public GameObject worldSpaceText; // Text indicator prefab (assign in Inspector)
    /*public float pickupRange = 2f;*/

    private bool isPlayerNearby = false;
    private bool isInspected = false;

    private void Start()
    {
        /*SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.radius = pickupRange;
        triggerCollider.isTrigger = true;*/
    }

    void Update()
    {

        if (InspectionManager.IsInspecting) return;

        // Toggle text based on proximity
        if (worldSpaceText != null )
            worldSpaceText.SetActive(isPlayerNearby);

        // Start inspection when interacting
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (worldSpaceText != null)
                worldSpaceText.SetActive(false);
            InspectionManager.Instance.StartInspection(itemData, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger zone: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone: " + other.name);
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left trigger zone: " + other.name);
            isPlayerNearby = false;
        }
    }

}
