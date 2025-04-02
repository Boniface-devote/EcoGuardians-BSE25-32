using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public RectTransform joystickBackground; // Reference to the joystick background
    public RectTransform joystickHandle; // Reference to the joystick handle
    private Vector2 joystickInput; // Store the joystick input
    private Vector2 joystickCenter; // Center of the joystick background
    private float joystickRadius; // Radius of the joystick background

    void Start()
    {
        // Calculate the center and radius of the joystick background
        joystickCenter = joystickBackground.position;
        joystickRadius = joystickBackground.sizeDelta.x / 2f; // Assuming the background is a square
    }

    void Update()
    {
        // If not dragging, reset input
        if (!Input.GetMouseButton(0))
        {
            joystickInput = Vector2.zero;
            joystickHandle.position = joystickCenter; // Reset handle position
        }

        // Move the player based on joystick input
        Vector3 movement = new Vector3(joystickInput.x, 0f, joystickInput.y) * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Rotate the player to face the movement direction
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }

    // Called when the joystick handle is dragged
    public void OnJoystickDrag(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;

        // Calculate the joystick input based on the drag position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, pointerData.position, pointerData.pressEventCamera, out localPoint);

        // Clamp the handle position within the background
        localPoint = Vector2.ClampMagnitude(localPoint, joystickRadius);
        joystickHandle.anchoredPosition = localPoint;

        // Calculate the normalized input
        joystickInput = localPoint / joystickRadius;
    }
}