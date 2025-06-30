using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorExperienceManager : MonoBehaviour
{

    [Header("General Config")]
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private int numberOfFloors = 5; // Default number of floors

    enum ElevatorType
    {
        Normal,
        Grid
    }

    enum ElevatorSize
    {
        OneByOne,
        TwoByOne,
        TwoByTwo
    }

    enum ElevatorState
    {
        Idle,
        MovingUp,
        MovingDown
    }

    [Header("Elevator Experience Configuration")]
    [SerializeField] private float elevatorSpeed = 2f; // Time between floors
    private float timeSinceLastFloorChange = 0f; // Timer to track time since last floor change
    [SerializeField] private float elevatorWaitTime = 2f; // Time to wait before after door starts closing
    private int targetFloor = 0;
    private int currentFloor = 0; // Current floor the elevator is on


    [Header("Elevator Configuration")]
    private ElevatorType currentElevatorType = ElevatorType.Grid;
    private GameObject currentElevatorObject;
    private GameObject currentElevatorChamber;
    private Animator currentElevatorAnimator;
    private ElevatorSize currentElevatorSize = ElevatorSize.TwoByTwo;
    private ElevatorState currentElevatorState = ElevatorState.Idle;

    [SerializeField] private TextMeshProUGUI elevatorTypeText;
    [SerializeField] private TextMeshProUGUI elevatorScaleText;
    [SerializeField] private TextMeshProUGUI elevatorFloorText;

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


    public IEnumerator GoToFloor(int floor)
    {
        yield return new WaitForSeconds(3f);
        if (floor < 0 || floor >= numberOfFloors)
        {
            Debug.LogError("Invalid floor number: " + floor);
            yield break; // Exit if the floor is invalid
        }



        if (currentElevatorState != ElevatorState.Idle)
        {
            Debug.LogWarning("Elevator is already moving.");

        }
        else
        {

            currentElevatorAnimator.SetTrigger("CloseDoor");
            yield return new WaitForSeconds(elevatorWaitTime);

            targetFloor = floor;

            if (targetFloor < currentFloor)
            {
                currentElevatorState = ElevatorState.MovingDown;
            }
            else if (targetFloor > currentFloor)
            {
                currentElevatorState = ElevatorState.MovingUp;
            }
            else
            {
                currentElevatorState = ElevatorState.Idle;
                Debug.Log("Elevator is already on the target floor: " + currentFloor);
                yield break; // Exit if already on the target floor
            }

        }



        if (floor == 0)
        {
            firstFloor.SetActive(true);
            genericFloor.SetActive(false);
        }
        else
        {
            firstFloor.SetActive(false);
            genericFloor.SetActive(true);
            floorManager.ApplyFloor(floor - 1); // Starts at index 0
        }



    }

    public void ToggleElevatorType()
    {
        if (currentElevatorType == ElevatorType.Normal)
        {
            currentElevatorType = ElevatorType.Grid;
            currentElevatorObject = gridElevator;
            currentElevatorChamber = gridElevatorChamber;
            currentElevatorAnimator = gridElevatorAnimator;
            normalElevator.SetActive(false);
            gridElevator.SetActive(true);
            //elevatorTypeText.text = "Normal Elevator";
        }
        else
        {
            currentElevatorType = ElevatorType.Normal;
            currentElevatorObject = normalElevator;
            currentElevatorChamber = normalElevatorChamber;
            currentElevatorAnimator = normalElevatorAnimator;
            gridElevator.SetActive(false);
            normalElevator.SetActive(true);
            //elevatorTypeText.text = "Grid Elevator";
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

        if(currentElevatorType == ElevatorType.Normal)
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

    

    public void CallElevator()
    {
        if (currentElevatorState == ElevatorState.Idle)
        {
            currentElevatorAnimator.SetTrigger("OpenDoor");
        }
    }

    public void CloseElevator()
    {
        if (currentElevatorState == ElevatorState.Idle)
        {
            currentElevatorAnimator.SetTrigger("CloseDoor");
        }
    }

    public void OpenElevatorOnNext()
    {
        if (currentElevatorState == ElevatorState.MovingUp)
        {
            targetFloor = currentFloor + 1;
        }
        else if (currentElevatorState == ElevatorState.MovingDown)
        {
            targetFloor = currentFloor - 1;
        }
        else
        {
            Debug.LogWarning("Elevator is not moving.");
            return; // Exit if the elevator is not moving
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        floorManager.Initalize(numberOfFloors);

        elevatorFloorText.text = currentFloor.ToString();

       

        ToggleElevatorType();
        ToggleElevatorType();

        //ChangeElevatorScale();
        //ChangeElevatorScale();

        //CallElevator();

        StartCoroutine(GoToFloor(4)); // Start at the first floor
    }

    // Update is called once per frame
    void Update()
    {
        if (currentElevatorState != ElevatorState.Idle)
        {
            timeSinceLastFloorChange += Time.deltaTime;
            if (timeSinceLastFloorChange > elevatorSpeed)
            {

                timeSinceLastFloorChange = 0;
                if (currentElevatorState == ElevatorState.MovingUp)
                {
                    currentFloor++;
                }
                else if (currentElevatorState == ElevatorState.MovingDown)
                {
                    currentFloor--;
                }

                elevatorFloorText.text = currentFloor.ToString();

                if (currentFloor < targetFloor)
                {
                    currentElevatorState = ElevatorState.MovingUp;
                }
                else if (currentFloor > targetFloor)
                {
                    currentElevatorState = ElevatorState.MovingDown;
                }
                else
                {
                    currentElevatorAnimator.SetTrigger("OpenDoor");
                    currentElevatorState = ElevatorState.Idle;

                    StartCoroutine(GoToFloor(0)); // Start at the first floor
                }

                Debug.Log("Elevator moved to floor: " + currentFloor);
            }
        }

    }
}
