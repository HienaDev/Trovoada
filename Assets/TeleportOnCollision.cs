using UnityEngine;
using UnityEngine.Events;

public class TeleportOnCollision : MonoBehaviour
{
    [SerializeField]
    private Transform teleportTarget;

    [SerializeField]
    private UnityEvent onTeleport;

    private void OnCollisionEnter(Collision collision)
    {
        Teleport(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Teleport(other.gameObject);
    }

    private void Teleport(GameObject obj)
    {
        if (teleportTarget == null)
        {
            Debug.LogWarning("Teleport target not assigned on " + gameObject.name);
            return;
        }

        // Teleport position and rotation
        obj.transform.position = teleportTarget.position;
        obj.transform.rotation = teleportTarget.rotation;

        // Reset rigidbody velocity if present
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Invoke UnityEvent
        onTeleport?.Invoke();
    }
}
