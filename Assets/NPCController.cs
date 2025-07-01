using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform assignedDoor;
    public Transform assignedElevator;

    public void MoveToDoor()
    {
        if (assignedDoor != null)
            agent.SetDestination(assignedDoor.position);
    }

    public void MoveToElevator()
    {
        if (assignedElevator != null)
            agent.SetDestination(assignedElevator.position);
    }
}
