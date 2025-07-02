using System.Collections;
using UnityEngine;

public class TeleportToTarget : MonoBehaviour
{
    public Transform target;

    IEnumerator Start()
    {

        yield return new WaitForSeconds(5f); // Wait for a short time to ensure the target is set

        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = target.rotation; // Optional: keep this if you also want to match rotation
        }
        else
        {
            Debug.LogWarning("TeleportToTarget: No target assigned.", this);
        }
    }
}
