using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorExperienceManager : MonoBehaviour
{

    [SerializeField] private FloorManager floorManager;

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

    private ElevatorType currentElevatorType = ElevatorType.Normal;
    private GameObject currentElevatorObject;
    private ElevatorSize currentElevatorSize = ElevatorSize.TwoByTwo;
    [SerializeField] private TextMeshProUGUI elevatorTypeText;
    [SerializeField] private GameObject normalElevator;
    [SerializeField] private Animator normalElevatorAnimator;
    [SerializeField] private GameObject gridElevator;
    [SerializeField] private Animator gridElevatorAnimator;

    [SerializeField] private Vector2 twoByTwoElevatorScale = new Vector2(1f, 1f);
    [SerializeField] private Vector2 twoByOneElevatorScale = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 oneByOneElevatorScale = new Vector2(0.5f, 0.5f);

    private int currentFloor = 0;   

    public void GoToFloor(int floor)
    {

    }

    public void ToggleElevatorType()
    {
        if (currentElevatorType == ElevatorType.Normal)
        {
            currentElevatorType = ElevatorType.Grid;
            currentElevatorObject = gridElevator;
            normalElevator.SetActive(false);
            gridElevator.SetActive(true);
            elevatorTypeText.text = "Normal Elevator";
        }
        else
        {
            currentElevatorType = ElevatorType.Normal;
            currentElevatorObject = normalElevator;
            gridElevator.SetActive(false);
            normalElevator.SetActive(true);
            elevatorTypeText.text = "Grid Elevator";
        }
    }

    public void ChangeElevatorScale()
    {

        if (currentElevatorSize == ElevatorSize.TwoByTwo)
        {
            currentElevatorObject.transform.localScale = new Vector3(twoByOneElevatorScale.x, 1f, twoByOneElevatorScale.y);
            currentElevatorSize = ElevatorSize.TwoByOne;
        }
        else if (currentElevatorSize == ElevatorSize.TwoByOne)
        {
            currentElevatorObject.transform.localScale = new Vector3(oneByOneElevatorScale.x, 1f, oneByOneElevatorScale.y);
            currentElevatorSize = ElevatorSize.OneByOne;
        }
        else
        {
            currentElevatorObject.transform.localScale = new Vector3(twoByTwoElevatorScale.x, 1f, twoByTwoElevatorScale.y);
            currentElevatorSize = ElevatorSize.TwoByTwo;
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
