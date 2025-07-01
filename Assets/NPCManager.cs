using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPCManager : MonoBehaviour
{
    public GameObject npcPrefab;

    public Transform[] doorPositions;
    public Transform[] elevatorPositions;

    private List<NPCController> activeNPCs = new List<NPCController>();
    private HashSet<int> usedElevatorIndices = new HashSet<int>();

    [SerializeField] private Vector2 npcSpeed = new Vector2(0.2f, 0.6f);

    private const int MAX_NPCS = 3;

    private int numberOfFloors;

    private Dictionary<int, List<NPCController>> npcFloorMap = new Dictionary<int, List<NPCController>>();


    public void Initialize(int floorNumber)
    {
        numberOfFloors = floorNumber;

        SpawnFromElevator();

        SpawnFromElevator();

        SpawnFromElevator();
    }

    void Update()
    {
        // For testing:
        if (Input.GetKeyDown(KeyCode.Alpha1)) SpawnFromElevator();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SpawnFromDoor();
        if (Input.GetKeyDown(KeyCode.Alpha3)) MoveAllToElevators();
        if (Input.GetKeyDown(KeyCode.Alpha4)) MoveAllToDoors();
    }



    public void SpawnFromElevator()
    {
        if (activeNPCs.Count >= MAX_NPCS) return;

        int elevatorIndex = GetFreeElevatorIndex();
        if (elevatorIndex == -1) return;

        Transform spawnPoint = elevatorPositions[elevatorIndex];
        Transform doorTarget = GetRandomDoor();

        GameObject npcGO = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        NPCController npc = npcGO.GetComponent<NPCController>();
        npc.agent = npcGO.GetComponent<NavMeshAgent>();
        npc.agent.speed = Random.Range(npcSpeed.x, npcSpeed.y);
        npc.assignedElevator = spawnPoint;
        npc.assignedDoor = doorTarget;

        npcGO.GetComponentInChildren<Animator>().SetTrigger(Random.Range(1 , 5) == 1 ? "Phone" : "Idle" );

        int targetFloor = Random.Range(1, numberOfFloors + 1);
        Debug.Log("Spawning NPC on floor: " + targetFloor);

        if (!npcFloorMap.ContainsKey(targetFloor))
        {
            npcFloorMap[targetFloor] = new List<NPCController>();
        }
        npcFloorMap[targetFloor].Add(npc);


        activeNPCs.Add(npc);
        usedElevatorIndices.Add(elevatorIndex);
    }

    public bool CheckIfShouldStopOnFloor(int floor)
    {
        return npcFloorMap.ContainsKey(floor) && npcFloorMap[floor].Count > 0;

    }

    public bool HasAnyNPCRequests()
    {
        foreach (var kvp in npcFloorMap)
        {
            if (kvp.Value != null && kvp.Value.Count > 0)
            {
                return true; // There is at least one NPC waiting somewhere
            }
        }
        return false; // No NPCs waiting anywhere
    }




    public void SpawnFromDoor()
    {
        if (activeNPCs.Count >= MAX_NPCS) return;

        Transform spawnPoint = GetRandomDoor();
        int elevatorIndex = GetFreeElevatorIndex();
        if (elevatorIndex == -1) return;

        Transform elevatorTarget = elevatorPositions[elevatorIndex];

        GameObject npcGO = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        npcGO.transform.eulerAngles = new Vector3(0, -90f, 0); 
        var npc = npcGO.GetComponent<NPCController>();
        npc.agent = npcGO.GetComponent<NavMeshAgent>();
        npc.assignedElevator = elevatorTarget;
        npc.assignedDoor = spawnPoint;

        activeNPCs.Add(npc);
        usedElevatorIndices.Add(elevatorIndex);
    }

    public void MoveNPCsToFloor(int floor)
    {
        if (!npcFloorMap.ContainsKey(floor)) return;

        List<NPCController> npcsToMove = npcFloorMap[floor];

        // Use a separate list to avoid modifying the collection while iterating
        List<NPCController> movedNPCs = new List<NPCController>();

        foreach (var npc in npcsToMove)
        {
            if (npc != null)
            {
                npc.MoveToDoor();
                npc.GetComponentInChildren<Animator>().SetTrigger("Walk");
                movedNPCs.Add(npc);
            }
        }

        // Remove only moved NPCs from the floor list
        foreach (var npc in movedNPCs)
        {
            npcFloorMap[floor].Remove(npc);
        }

    }



    public void MoveAllToElevators()
    {
        foreach (var npc in activeNPCs)
        {
            npc.MoveToElevator();
        }
    }

    public void MoveAllToDoors()
    {
        foreach (var npc in activeNPCs)
        {
            npc.MoveToDoor();
        }
    }

    private Transform GetRandomDoor()
    {
        int index = Random.Range(0, doorPositions.Length);
        return doorPositions[index];
    }

    private int GetFreeElevatorIndex()
    {
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < elevatorPositions.Length; i++)
        {
            if (!usedElevatorIndices.Contains(i))
                freeIndices.Add(i);
        }

        if (freeIndices.Count == 0) return -1;

        return freeIndices[Random.Range(0, freeIndices.Count)];
    }

    public void RemoveNPC(NPCController npc)
    {
        foreach (var kvp in npcFloorMap)
        {
            if (kvp.Value.Contains(npc))
            {
                kvp.Value.Remove(npc);
                if (kvp.Value.Count == 0)
                {
                    npcFloorMap.Remove(kvp.Key);
                }
                break;
            }
        }


        int index = System.Array.IndexOf(elevatorPositions, npc.assignedElevator);
        if (index >= 0) usedElevatorIndices.Remove(index);

        activeNPCs.Remove(npc);
        Destroy(npc.gameObject);
    }
}
