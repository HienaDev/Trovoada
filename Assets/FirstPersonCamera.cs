using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity of the mouse movement
    public Transform playerBody;          // Reference to the player's body for horizontal rotation
    private float xRotation = 0f;         // Tracks the up/down rotation (vertical)
    private float yRotation = 0f;         // Tracks the horizontal rotation (locked if needed)

    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate the vertical rotation (clamped to avoid flipping the camera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        // Apply the vertical rotation (up/down) to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Optionally lock Y-axis rotation
        // This prevents the player body from rotating on the Y-axis
        yRotation += mouseX;
        //yRotation = Mathf.Clamp(yRotation, -60f - 90f, 60f - 90f); // Locking Y-rotation within a specific range (optional)

        // Apply the rotation to the player body (only on the Y-axis if needed)
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
