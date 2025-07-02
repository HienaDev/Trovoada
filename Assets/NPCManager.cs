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

    // Reference to the elevator manager to register floors
    private ElevatorExperienceManager elevatorManager;

    // Dictionary to track NPCs waiting on each floor
    private Dictionary<int, List<NPCController>> npcFloorMap = new Dictionary<int, List<NPCController>>();

    public void Initialize(int floorNumber, ElevatorExperienceManager elevator)
    {
        numberOfFloors = floorNumber;
        elevatorManager = elevator;

    }

    void Update()
    {

    }

    public void SpawnFromElevator()
    {
        foreach (NPCController npcActive in activeNPCs)
        {
            if (npcActive == null || npcActive.gameObject == null)
            {
                RemoveNPC(npcActive);
                return; // Exit early if we find a null NPC
            }
        }

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

        npcGO.GetComponentInChildren<Animator>().SetTrigger(Random.Range(1, 5) == 1 ? "Phone" : "Idle");

        // Generate target floor and register it with the elevator
        int targetFloor = Random.Range(1, numberOfFloors);
        npc.targetFloor = targetFloor; // Store target floor in NPC

        Debug.Log($"Spawning NPC with target floor: {targetFloor}");

        // Add NPC to floor tracking
        if (!npcFloorMap.ContainsKey(targetFloor))
        {
            npcFloorMap[targetFloor] = new List<NPCController>();
        }
        npcFloorMap[targetFloor].Add(npc);

        // Register the floor with the elevator manager
        elevatorManager.RegisterFloor(targetFloor);

        activeNPCs.Add(npc);
        usedElevatorIndices.Add(elevatorIndex);
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

        // Generate target floor and register it with the elevator
        int targetFloor = Random.Range(1, numberOfFloors + 1);
        npc.targetFloor = targetFloor;

        Debug.Log($"Spawning NPC from door with target floor: {targetFloor}");

        // Add NPC to floor tracking
        if (!npcFloorMap.ContainsKey(targetFloor))
        {
            npcFloorMap[targetFloor] = new List<NPCController>();
        }
        npcFloorMap[targetFloor].Add(npc);

        // Register the floor with the elevator manager
        elevatorManager.RegisterFloor(targetFloor);

        activeNPCs.Add(npc);
        usedElevatorIndices.Add(elevatorIndex);
    }

    // This method is now only used for moving NPCs when elevator reaches their floor
    public void MoveNPCsToFloor(int floor)
    {
        if (!npcFloorMap.ContainsKey(floor)) return;

        List<NPCController> npcsToMove = npcFloorMap[floor];
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

        // Remove moved NPCs from the floor list
        foreach (var npc in movedNPCs)
        {
            npcFloorMap[floor].Remove(npc);
        }

        // Clean up empty floor entries
        if (npcFloorMap[floor].Count == 0)
        {
            npcFloorMap.Remove(floor);
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
        // Remove from floor mapping
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

        // Free up elevator position
        int index = System.Array.IndexOf(elevatorPositions, npc.assignedElevator);
        if (index >= 0) usedElevatorIndices.Remove(index);

        activeNPCs.Remove(npc);

        if(npc.gameObject != null)
            Destroy(npc.gameObject);
    }

    // Optional: Method to check if there are NPCs waiting on a specific floor
    // (kept for compatibility but shouldn't be needed with direct registration)
    public bool HasNPCsOnFloor(int floor)
    {
        return npcFloorMap.ContainsKey(floor) && npcFloorMap[floor].Count > 0;
    }
}