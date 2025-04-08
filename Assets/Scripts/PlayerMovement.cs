using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 6f;
    public float horizontalSpeed = 3f;
    public float rightLimit = 5.5f;
    public float leftLimit = -5.5f;

    private Vector2 moveInput; // Stores input values

    void Update()
    {
        // Get input from keyboard
        float moveX = 0;
        float moveZ = 1; // Default forward movement

        if (Keyboard.current != null)
        {
            moveX = (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
                    (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0);

            moveZ = (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? -1 : 1);
        }

        // Get input from joystick
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
            moveX = moveInput.x;
            moveZ = moveInput.y;
        }

        // Get current position
        Vector3 newPosition = transform.position;

        // Horizontal movement (Left/Right)
        if (moveX < 0 && newPosition.x > leftLimit) // Moving left
        {
            newPosition.x -= horizontalSpeed * Time.deltaTime;
        }
        else if (moveX > 0 && newPosition.x < rightLimit) // Moving right
        {
            newPosition.x += horizontalSpeed * Time.deltaTime;
        }

        // Forward/Backward movement (Joystick or Keyboard)
        newPosition.z += moveZ * playerSpeed * Time.deltaTime;

        // Apply the movement
        transform.position = newPosition;
    }
}
