using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Movement speed

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get input for movement (WASD or arrow keys)
        float moveHorizontal = Input.GetAxis("Horizontal"); // Left/Right movement
        float moveVertical = Input.GetAxis("Vertical");     // Forward/Backward movement

        // Combine input into a local movement vector
        Vector3 localMovement = new Vector3(moveHorizontal, 0f, moveVertical);

        // Convert local movement to world space using the player's rotation
        Vector3 worldMovement = transform.TransformDirection(localMovement).normalized;

        // Apply velocity to Rigidbody
        rb.velocity = worldMovement * speed + new Vector3(0f, rb.velocity.y, 0f); // Preserve vertical velocity (gravity)
    }
}
