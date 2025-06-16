using UnityEngine;

public class SmoothRotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationSpeed = new Vector3(30f, 45f, 60f);

    [SerializeField]
    private float bobAmplitude = 0.5f; // How high it bobs
    [SerializeField]
    private float bobFrequency = 1f;   // How fast it bobs

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Rotate the object
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Bob up and down using a sine wave
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
