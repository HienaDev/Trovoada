using UnityEngine;

public class VerticalMover : MonoBehaviour
{
    public enum MoveDirection { Up, Down, Still }
    public MoveDirection direction = MoveDirection.Still;

    public float speed = 2f;

    private MoveDirection lastDirection;

    [SerializeField] private ControlGridWall controlGridWall;

    public void Initalize(float elevatorSpeed)
    {
        speed = 1 / elevatorSpeed * 5.3f;

    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        switch (direction)
        {
            case MoveDirection.Up:
                if(lastDirection != MoveDirection.Up)
                {
                    controlGridWall.ResetPositions();
                    lastDirection = MoveDirection.Up;
                    Debug.Log("Moving Up");
                }
                movement = Vector3.up;
                break;
            case MoveDirection.Down:
                if (lastDirection != MoveDirection.Down)
                {
                    controlGridWall.ResetPositions();
                    lastDirection = MoveDirection.Down;
                    Debug.Log("Moving Down");
                }
                movement = Vector3.down;
                break;
            case MoveDirection.Still:
                
                movement = Vector3.zero;
                break;
        }

        transform.Translate(movement * speed * 0.18f * Time.deltaTime); // 0.18f is the scale of the elevator, the only scaled parent, we do this to normalize the speed
                                                                        // Now with 1 speed we go through 1 floor in 1 second, so we want the speed to be:
                                                                        // 1 divided by Time for elevator to travel 1 floor, each wall needs to go up 5.3f units
                                                                        // to be the new floor, so we need to add that to the speed calculations, these are all 
                                                                        // very hardcoded values (bad :( )
    }
}
