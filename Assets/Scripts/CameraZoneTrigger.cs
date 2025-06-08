using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    public GameObject fixedCamera; // Assign the fixed camera for this zone in the Inspector
    public PlayerController playerController; // Reference to the PlayerController script
    private Quaternion preservedCharacterRotation;
    private Vector3 lastKnownDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            // Activate this fixed camera
            fixedCamera.SetActive(true);

            // Enable fixed camera mode in the player controller
            playerController.isUsingFixedCamera = true;

            // Ensure the original camera is disabled
            playerController.worldReferenceOrientation = fixedCamera.transform; // Update reference
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deactivate this fixed camera
            fixedCamera.SetActive(false);

            // Check if the player is still in another fixed camera zone
            CameraZoneTrigger[] otherZones = FindObjectsOfType<CameraZoneTrigger>();
            bool playerInAnotherZone = false;

            foreach (var zone in otherZones)
            {
                if (zone != this && zone.fixedCamera.activeSelf)
                {
                    playerInAnotherZone = true;
                    break;
                }
            }

            if (!playerInAnotherZone)
            {
                playerController.SyncOrientationToPlayerModel();
                // Revert to the top-down camera if no other zones are active
                playerController.isUsingFixedCamera = false;
            }
        }
    }
}
