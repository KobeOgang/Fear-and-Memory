using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float normalMoveSpeed;
    public float sprintSpeed = 10f;

    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask WhatIsGround;
    bool isGrounded;

    public Transform playerModel; // Reference to the player model
    public Transform orientation; // Reference to the active camera's orientation
    public Transform worldReferenceOrientation; // Reference for fixed camera zones

    public float rotationSpeed = 5f; // Speed of mouse rotation
    public bool isUsingFixedCamera = false; // Flag to check if a fixed camera is active

    float hInput;
    float vInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        // This prevents the player from rotating or moving in the background.
        if (InspectionManager.IsInspecting)
        {
            return; // Exit the Update method immediately
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, WhatIsGround);
        PlayerInput();
        SpeedControl();

        // Handle ground drag
        rb.drag = isGrounded ? groundDrag : 0f;

        // Rotate player based on active camera mode
        if (isUsingFixedCamera)
        {
            RotatePlayerWithMouse(); // Mouse-based rotation for fixed cameras
        }
        else
        {
            SmoothFaceCamera(); // Smooth rotation relative to the top-down camera
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInput()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = normalMoveSpeed;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction based on the active mode
        if (isUsingFixedCamera)
        {
            // Movement relative to the player's current facing direction
            moveDirection = playerModel.forward * vInput + playerModel.right * hInput;
            // The 'worldReferenceOrientation' is set by the CameraZoneTrigger script.
            //moveDirection = worldReferenceOrientation.forward * vInput + worldReferenceOrientation.right * hInput;
        }
        else
        {
            // Movement relative to the active camera's orientation
            moveDirection = orientation.forward * vInput + orientation.right * hInput;
        }

        moveDirection.y = 0; // Ensure movement is horizontal

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void SmoothFaceCamera()
    {
        if (!isUsingFixedCamera)
        {
            // Smooth rotation to face the active camera's orientation
            float targetAngle = orientation.eulerAngles.y;
            float smoothedAngle = Mathf.LerpAngle(playerModel.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);
            playerModel.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }
    }

    private void RotatePlayerWithMouse()
    {
        // Allow player rotation with mouse input
        float mouseX = Input.GetAxis("Mouse X");
        playerModel.Rotate(Vector3.up, mouseX * rotationSpeed);

    }
    public void SyncOrientationToPlayerModel()
    {
        orientation.rotation = playerModel.rotation;
    }

}
