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
        // Move forward continuously
        transform.Translate(Vector3.forward * Time.deltaTime * playerSpeed, Space.World);

        // Get input from the new Input System
        moveInput = Keyboard.current != null ? new Vector2(
            (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1 : 0) +
            (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1 : 0),
            0) : Vector2.zero;

        // Get current position
        Vector3 newPosition = transform.position;

        // Check movement within limits
        if (moveInput.x < 0 && newPosition.x > leftLimit) // Moving left
        {
            newPosition.x -= horizontalSpeed * Time.deltaTime;
        }
        else if (moveInput.x > 0 && newPosition.x < rightLimit) // Moving right
        {
            newPosition.x += horizontalSpeed * Time.deltaTime;
        }

        // Apply the movement
        transform.position = newPosition;
    }
}
