using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGridWall : MonoBehaviour
{

    [SerializeField] private Transform goingStartUpPosition;
    [SerializeField] private Transform goingStartDownPosition;

    [SerializeField] private VerticalMover.MoveDirection directionToTrigger = VerticalMover.MoveDirection.Up;

    [SerializeField] private VerticalMover verticalMover;

    [SerializeField] private Transform wall1;
    private Vector3 wall1StartPosition;
    [SerializeField] private Transform wall2;
    private Vector3 wall2StartPosition;

    // Start is called before the first frame update
    void Start()
    {
        wall1StartPosition = wall1.position;
        wall2StartPosition = wall2.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GridWall"))
        {
            
            if(VerticalMover.MoveDirection.Up == verticalMover.direction && directionToTrigger == VerticalMover.MoveDirection.Up)
                other.transform.position = goingStartUpPosition.position;
            else if (VerticalMover.MoveDirection.Down == verticalMover.direction && directionToTrigger == VerticalMover.MoveDirection.Down)
            {
                other.transform.position = goingStartDownPosition.position;
            }
        }
    }

    public void ResetPositions()
    {
        wall1.position = wall1StartPosition;
        wall2.position = wall2StartPosition;
    }
}
