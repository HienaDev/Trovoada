using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportOnCollision : MonoBehaviour
{
    [SerializeField]
    private Transform teleportTarget;
    [SerializeField]
    private UnityEvent onTeleport;
    [SerializeField] private ImageFader imageFader;
    private Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " collided with " + gameObject.name);
        collider.enabled = false;
        imageFader.FadeInThenEventThenFadeOut();
        imageFader.onFadeInComplete.AddListener(() => Teleport(collision.gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " triggered " + gameObject.name);
        collider.enabled = false;
        imageFader.FadeInThenEventThenFadeOut();
        imageFader.onFadeInComplete.AddListener(() => Teleport(other.gameObject));
        imageFader.onFadeOutComplete.AddListener(() => collider.enabled = true);
    }

    private void Teleport(GameObject obj)
    {
        if (teleportTarget == null)
        {
            Debug.LogWarning("Teleport target not assigned on " + gameObject.name);
            return;
        }

        XROrigin xrOrigin = obj.GetComponent<XROrigin>();
        if (xrOrigin != null)
        {
            // Find and disable the Continuous Move Provider
            var continuousMoveProvider = FindObjectOfType<ActionBasedContinuousMoveProvider>();
            bool wasEnabled = false;

            if (continuousMoveProvider != null)
            {
                wasEnabled = continuousMoveProvider.enabled;
                continuousMoveProvider.enabled = false;
            }

            // Now teleport directly
            obj.transform.position = teleportTarget.position;
            //obj.transform.rotation = teleportTarget.rotation;

            // Re-enable after a short delay
            if (continuousMoveProvider != null)
            {
                StartCoroutine(ReEnableMovement(continuousMoveProvider));
            }
        }
        else
        {
            // Regular object
            obj.transform.position = teleportTarget.position;
            //obj.transform.rotation = teleportTarget.rotation;
        }

        // Reset rigidbody velocity
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


        onTeleport?.Invoke();
    }

    private System.Collections.IEnumerator ReEnableMovement(ActionBasedContinuousMoveProvider moveProvider)
    {
        // Wait a frame or two
        yield return new WaitForSeconds(0.1f);

        if (moveProvider != null)
        {
            moveProvider.enabled = true;
        }
    }
}