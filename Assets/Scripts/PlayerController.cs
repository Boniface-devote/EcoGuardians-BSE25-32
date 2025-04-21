using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Normal movement speed
    public float sprintMultiplier = 2f; // Multiplier for sprinting
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    private Vector2 joystickInput;
    private Vector2 joystickCenter;
    private float joystickRadius;
    private bool isSprinting = false; // Sprint state

    void Start()
    {
        joystickCenter = joystickBackground.position;
        joystickRadius = joystickBackground.sizeDelta.x / 2f;
    }

    void Update()
    {
        // Handle sprinting with keyboard
        isSprinting = Input.GetKey(KeyCode.LeftShift) || isSprinting;

        // Reset joystick when not dragging
        if (!Input.GetMouseButton(0))
        {
            joystickInput = Vector2.zero;
            joystickHandle.position = joystickCenter;
        }

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        // Move player
        Vector3 movement = new Vector3(joystickInput.x, 0f, joystickInput.y) * currentSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Rotate player
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }

        // Reset isSprinting if not holding shift or UI button
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
    }

    public void OnJoystickDrag(BaseEventData eventData)
    {
        PointerEventData pointerData = (PointerEventData)eventData;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, pointerData.position, pointerData.pressEventCamera, out localPoint);

        localPoint = Vector2.ClampMagnitude(localPoint, joystickRadius);
        joystickHandle.anchoredPosition = localPoint;

        joystickInput = localPoint / joystickRadius;
    }

    // Called by the UI Sprint Button (OnPointerDown)
    public void StartSprinting()
    {
        isSprinting = true;
    }

    // Called by the UI Sprint Button (OnPointerUp)
    public void StopSprinting()
    {
        isSprinting = false;
    }
}
