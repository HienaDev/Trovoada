using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform assignedElevator;
    [HideInInspector] public Transform assignedDoor;
    [HideInInspector] public int targetFloor; // The floor this NPC wants to go to

    public float reachThreshold = 0.5f; // Radius threshold for reaching destination

    private Transform currentTarget;

    private void Update()
    {
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);
        Debug.Log($"{gameObject.name} distance to target: {distance}");

        if (distance <= reachThreshold)
        {
            Debug.Log($"{gameObject.name} reached target by distance and will be destroyed.");
            Destroy(gameObject);
        }
    }

    public void MoveToElevator()
    {
        if (agent != null && assignedElevator != null)
        {
            currentTarget = assignedElevator;
            agent.SetDestination(assignedElevator.position);
        }
    }

    public void MoveToDoor()
    {
        if (agent != null && assignedDoor != null)
        {
            currentTarget = assignedDoor;
            agent.SetDestination(assignedDoor.position);
        }
    }

    public int GetTargetFloor()
    {
        return targetFloor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, reachThreshold);
    }
}
