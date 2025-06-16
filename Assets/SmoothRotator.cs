using UnityEngine;

public class SmoothRotator : MonoBehaviour
{
    // Rotation speed in degrees per second
    public Vector3 rotationSpeed = new Vector3(30f, 45f, 60f);

    void Update()
    {
        // Rotate the object each frame
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
