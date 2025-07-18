﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorExperienceManager : MonoBehaviour
{
    [Header("General Config")]
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private int numberOfFloors = 5;

    enum ElevatorType { Normal, Grid }
    public enum ElevatorSize { OneByOne, TwoByOne, TwoByTwo }
    enum ElevatorState { Idle, MovingUp, MovingDown, DoorsOpening, DoorsClosing, WaitingAtFloor }
    enum Direction { None, Up, Down }

    [Header("Elevator Experience Configuration")]
    [SerializeField] private float elevatorSpeed = 2f;
    [SerializeField] private float elevatorWaitTime = 2f;
    [SerializeField] private float doorOperationTime = 1f;

    // Floor queue management
    private HashSet<int> registeredFloors = new HashSet<int>();
    private int currentFloor = 0;
    private Direction currentDirection = Direction.None;
    private ElevatorState currentElevatorState = ElevatorState.Idle;

    // Timers
    private float timeSinceLastFloorChange = 0f;
    private float doorTimer = 0f;
    private float waitTimer = 0f;

    [Header("Elevator Configuration")]
    private ElevatorType currentElevatorType = ElevatorType.Grid;
    private GameObject currentElevatorObject;
    private GameObject currentElevatorChamber;
    private Animator currentElevatorAnimator;
    private ElevatorSize currentElevatorSize = ElevatorSize.TwoByTwo;
    [SerializeField] private VerticalMover gridElevatorWallMover;

    [SerializeField] private TextMeshProUGUI elevatorTypeText;
    [SerializeField] private TextMeshProUGUI elevatorScaleText;
    [SerializeField] private TextMeshProUGUI elevatorFloorText;
    [SerializeField] private TextMeshProUGUI elevatorMusicText;

    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource audioDing;
    [SerializeField] private AudioSource elevatorTravelSound1;
    [SerializeField] private AudioSource elevatorTravelSound2;

    [SerializeField] private Transform buttons;
    [SerializeField] private Transform floorVisualizer;

    [SerializeField] private GameObject normalElevator;
    [SerializeField] private GameObject normalElevatorChamber;
    [SerializeField] private Animator normalElevatorAnimator;
    [SerializeField] private Transform normalElevatorButtonPosition;
    [SerializeField] private Transform normalElevatorFloorVisualizerPosition;

    [SerializeField] private GameObject gridElevator;
    [SerializeField] private GameObject gridElevatorChamber;
    [SerializeField] private Animator gridElevatorAnimator;
    [SerializeField] private Transform gridElevatorButtonPosition;
    [SerializeField] private Transform gridElevatorFloorVisualizerPosition;

    [SerializeField] private GameObject firstFloor;
    [SerializeField] private GameObject genericFloor;

    [Header("Elevator Scales")]
    [SerializeField] private Vector2 twoByTwoElevatorScale = new Vector2(1f, 1f);
    [SerializeField] private Vector2 twoByOneElevatorScale = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 oneByOneElevatorScale = new Vector2(0.5f, 0.5f);

    [SerializeField] private MeshRenderer[] buttonMeshes;
    [SerializeField] private Material buttonActiveMaterial;
    [SerializeField] private Material buttonInactiveMaterial;

    [Header("NPC Manager")]
    [SerializeField] private NPCManager npcManager;

    private bool playerInsideElevator = false;

    public void RegisterFloorNormal(int floor)
    {
        RegisterFloor(floor);
    }

    public void RegisterFloor(int floor, bool NPC = false)
    {
        if (floor < 0 || floor >= numberOfFloors)
        {
            Debug.Log($"Invalid floor number: {floor}");
            return;
        }

        if (floor == currentFloor && currentElevatorState == ElevatorState.Idle)
        {
            Debug.Log($"Already at floor {floor}, opening doors");
            StartDoorOperation(true);
            return;
        }

        registeredFloors.Add(floor);
        Debug.Log($"Registered floor {floor}. Queue: [{string.Join(", ", registeredFloors.OrderBy(f => f))}]");
        buttonMeshes[floor].material = buttonActiveMaterial;

        // Start moving if idle and wasn't called by NPC
        if (currentElevatorState == ElevatorState.Idle && !NPC)
        {
            Debug.Log("Starting elevator movement from idle state");
            StartDoorOperation(false);
            currentElevatorState = ElevatorState.DoorsClosing;
        }
    }

    public void UnregisterFloor(int floor)
    {
        if (floor < 0 || floor >= numberOfFloors)
        {
            Debug.Log($"Invalid floor number: {floor}");
            return;
        }

        if (!registeredFloors.Contains(floor))
        {
            Debug.Log($"Floor {floor} is not registered, cannot unregister.");
            return;
        }

        registeredFloors.Remove(floor);
        Debug.Log($"Remove registered floor: " + floor);
        buttonMeshes[floor].material = buttonInactiveMaterial;
    }

    public void ForceOpenElevatorOnNextFloor()
    {
        if (currentElevatorState == ElevatorState.MovingUp && currentFloor < numberOfFloors)
        {
            RegisterFloor(currentFloor + 1);
        }
        else if (currentElevatorState == ElevatorState.MovingDown && currentFloor > 0)
        {
            RegisterFloor(currentFloor - 1);
        }
        else
        {
            Debug.LogWarning("Cannot force open elevator when not moving.");
        }


    }

    public void CallElevator()
    {
        if (currentElevatorState == ElevatorState.Idle)
        {
            StartDoorOperation(true, true);
        }
    }

    public void CloseElevator()
    {
        if (currentElevatorState == ElevatorState.Idle)
        {
            Debug.Log("Close elevator");
            StartDoorOperation(false, true);
        }
    }

    // Core elevator logic
    private void StartMoving()
    {
        elevatorTravelSound1.volume = 1f;
        elevatorTravelSound2.volume = 1f;

        if (registeredFloors.Count == 0)
        {
            currentElevatorState = ElevatorState.Idle;
            currentDirection = Direction.None;
            return;
        }

        Direction newDirection = DetermineDirection();
        if (newDirection == Direction.None)
        {
            currentElevatorState = ElevatorState.Idle;
            return;
        }

        currentDirection = newDirection;
        currentElevatorState = currentDirection == Direction.Up ? ElevatorState.MovingUp : ElevatorState.MovingDown;

        Debug.Log($"Starting to move {currentDirection} from floor {currentFloor}");
    }

    private Direction DetermineDirection()
    {
        if (registeredFloors.Count == 0) return Direction.None;

        bool hasFloorsAbove = registeredFloors.Any(f => f > currentFloor);
        bool hasFloorsBelow = registeredFloors.Any(f => f < currentFloor);

        // Continue in current direction if possible, otherwise switch
        if (currentDirection == Direction.Up && hasFloorsAbove)
            return Direction.Up;
        else if (currentDirection == Direction.Down && hasFloorsBelow)
            return Direction.Down;
        else if (hasFloorsAbove)
            return Direction.Up;
        else if (hasFloorsBelow)
            return Direction.Down;

        return Direction.None;
    }

    private bool ShouldStopAtCurrentFloor()
    {
        // Always stop if this floor is registered
        return registeredFloors.Contains(currentFloor);
    }

    private void StopAtFloor()
    {
        Debug.Log($"Stopping at floor {currentFloor}");

        // Remove this floor from registered floors
        UnregisterFloor(currentFloor);

        // Apply the floor visual
        ApplyFloorVisual(currentFloor);

        // Start door opening sequence
        doorTimer = 0f;
        StartDoorOperation(true);
    }

    private void ApplyFloorVisual(int floor)
    {
        if (floor == 0)
        {
            firstFloor.SetActive(true);
            genericFloor.SetActive(false);
        }
        else
        {
            firstFloor.SetActive(false);
            genericFloor.SetActive(true);
            floorManager.ApplyFloor(floor - 1);
        }
    }

    private void StartDoorOperation(bool opening, bool force = false)
    {
        if (opening)
        {
            audioDing.Play();
            elevatorTravelSound1.volume = 0f;
            elevatorTravelSound2.volume = 0f;

            if (!force)
                currentElevatorState = ElevatorState.DoorsOpening;

            currentElevatorAnimator.SetTrigger("OpenDoor");
            currentElevatorAnimator.ResetTrigger("CloseDoor");
        }
        else
        {
            if (!force)
                currentElevatorState = ElevatorState.DoorsClosing;

            currentElevatorAnimator.SetTrigger("CloseDoor");
            currentElevatorAnimator.ResetTrigger("OpenDoor");
        }
        doorTimer = 0f;
    }

    private bool HasMoreDestinations()
    {
        return registeredFloors.Count > 0;
    }

    // Update method - handles all elevator logic
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            CallElevator();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            RegisterFloor(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RegisterFloor(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RegisterFloor(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RegisterFloor(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RegisterFloor(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            RegisterFloor(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            RegisterFloor(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            RegisterFloor(7);
        }

        if (!playerInsideElevator)
            return;

        switch (currentElevatorState)
        {
            case ElevatorState.MovingUp:
            case ElevatorState.MovingDown:
                HandleMovement();
                break;

            case ElevatorState.DoorsOpening:
                gridElevatorWallMover.direction = VerticalMover.MoveDirection.Still;
                HandleDoorOpening();
                break;

            case ElevatorState.DoorsClosing:
                gridElevatorWallMover.direction = VerticalMover.MoveDirection.Still;
                HandleDoorClosing();
                break;

            case ElevatorState.WaitingAtFloor:
                gridElevatorWallMover.direction = VerticalMover.MoveDirection.Still;
                HandleWaitingAtFloor();
                break;
        }
    }

    public void TogglePlayerLocation(bool toggle)
    {
        playerInsideElevator = toggle;
        if(!playerInsideElevator)
        {
            currentElevatorState = ElevatorState.Idle;
        }
        else
        {
            currentElevatorState = ElevatorState.WaitingAtFloor;
        }
        waitTimer = 0f;
        Debug.Log($"Player inside elevator: {playerInsideElevator}");
    }

    private void HandleMovement()
    {
        timeSinceLastFloorChange += Time.deltaTime;

        if(currentElevatorState == ElevatorState.MovingUp)
            gridElevatorWallMover.direction = VerticalMover.MoveDirection.Down;
        else if (currentElevatorState == ElevatorState.MovingDown)
            gridElevatorWallMover.direction = VerticalMover.MoveDirection.Up;

        if (timeSinceLastFloorChange >= elevatorSpeed)
        {
            timeSinceLastFloorChange = 0f;

            // Move to next floor
            if (currentElevatorState == ElevatorState.MovingUp)
            {
                currentFloor++;
            }

            else if (currentElevatorState == ElevatorState.MovingDown)
            {
                currentFloor--;
            }


            elevatorFloorText.text = currentFloor.ToString();
            Debug.Log($"Elevator moved to floor: {currentFloor}");

            // Check if we should stop
            if (ShouldStopAtCurrentFloor())
            {
                StopAtFloor();
            }
        }
    }

    private void HandleDoorOpening()
    {
        doorTimer += Time.deltaTime;

        if (doorTimer >= doorOperationTime)
        {
            // Doors are now open
            currentElevatorState = ElevatorState.WaitingAtFloor;
            waitTimer = 0f;

            // Handle NPC movement
            npcManager.MoveNPCsToFloor(currentFloor);
            buttonMeshes[currentFloor].material = buttonInactiveMaterial;

            Debug.Log($"Doors opened at floor {currentFloor}");
        }
    }

    private void HandleDoorClosing()
    {
        doorTimer += Time.deltaTime;

        if (doorTimer >= doorOperationTime)
        {
            // Doors are now closed
            Debug.Log($"Doors closed at floor {currentFloor}");

            // Determine next action
            if (HasMoreDestinations())
            {
                StartMoving();
            }
            else
            {
                currentElevatorState = ElevatorState.Idle;
                currentDirection = Direction.None;
                Debug.Log("Elevator is now idle - no more destinations");
            }
        }
    }

    private void HandleWaitingAtFloor()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= elevatorWaitTime)
        {
            // Time to close doors
            if (HasMoreDestinations())
            {
                StartDoorOperation(false);
            }
            else
            {
                Debug.Log("No more destinations - keeping elevator door open");
                currentElevatorState = ElevatorState.Idle;
                currentDirection = Direction.None;
            }
        }
    }

    // Existing methods (kept unchanged)
    public void ToggleElevatorType()
    {
        if (currentElevatorType == ElevatorType.Normal)
        {
            currentElevatorType = ElevatorType.Grid;
            currentElevatorObject = gridElevator;
            currentElevatorChamber = gridElevatorChamber;
            currentElevatorAnimator = gridElevatorAnimator;
            elevatorTypeText.text = "Tipo: Grelha";
            normalElevator.SetActive(false);
            gridElevator.SetActive(true);
        }
        else
        {
            currentElevatorType = ElevatorType.Normal;
            currentElevatorObject = normalElevator;
            currentElevatorChamber = normalElevatorChamber;
            currentElevatorAnimator = normalElevatorAnimator;
            elevatorTypeText.text = "Tipo: Normal";
            gridElevator.SetActive(false);
            normalElevator.SetActive(true);
        }
    }

    public void ChangeElevatorScale()
    {
        if (currentElevatorSize == ElevatorSize.TwoByTwo)
        {
            currentElevatorChamber.transform.localScale = new Vector3(twoByOneElevatorScale.x, 1f, twoByOneElevatorScale.y);
            currentElevatorSize = ElevatorSize.TwoByOne;
            elevatorScaleText.text = "Escala: 2x1";
        }
        else if (currentElevatorSize == ElevatorSize.TwoByOne)
        {
            currentElevatorChamber.transform.localScale = new Vector3(oneByOneElevatorScale.x, 1f, oneByOneElevatorScale.y);
            currentElevatorSize = ElevatorSize.OneByOne;
            elevatorScaleText.text = "Escala: 1x1";
        }
        else
        {
            currentElevatorChamber.transform.localScale = new Vector3(twoByTwoElevatorScale.x, 1f, twoByTwoElevatorScale.y);
            currentElevatorSize = ElevatorSize.TwoByTwo;
            elevatorScaleText.text = "Escala: 2x2";
        }

        StartCoroutine(npcManager.ChangeNPCPositions(currentElevatorSize));

        if (currentElevatorType == ElevatorType.Normal)
        {
            buttons.position = normalElevatorButtonPosition.position;
            floorVisualizer.position = normalElevatorFloorVisualizerPosition.position;
        }
        else
        {
            buttons.position = gridElevatorButtonPosition.position;
            floorVisualizer.position = gridElevatorFloorVisualizerPosition.position;
        }
    }

    public void ToggleMusic()
    {
        music.enabled = !music.enabled;
        if (music.enabled)
        {
            elevatorMusicText.text = "Música: Ligada";
        }
        else
        {
            elevatorMusicText.text = "Música: Desligada";
        }
    }

    void Start()
    {
        floorManager.Initalize(numberOfFloors);
        npcManager.Initialize(numberOfFloors, this);
        gridElevatorWallMover.Initalize(elevatorSpeed);

        elevatorFloorText.text = currentFloor.ToString();

        ToggleElevatorType();
        ToggleElevatorType();
    }
}