using UnityEngine;
using UnityEngine.Events;

public class CollisionAndTriggerEvent : MonoBehaviour
{
    [Tooltip("Called when a collision or trigger occurs.")]
    public UnityEvent onCollisionOrTrigger;

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionOrTrigger?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        onCollisionOrTrigger?.Invoke();
    }
}
