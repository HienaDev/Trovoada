using UnityEngine;

public class TeleportOnCollision : MonoBehaviour
{
    [SerializeField]
    private Transform teleportTarget;

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

        // Teleport position
        obj.transform.position = teleportTarget.position;

        // Teleport rotation
        obj.transform.rotation = teleportTarget.rotation;


    }
}
