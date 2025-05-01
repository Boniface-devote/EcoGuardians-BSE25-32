using UnityEngine;
using UnityEngine.EventSystems;

public class TruckController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float steeringAngle = 30f;
    public float sprintMultiplier = 1.5f;

    [Header("Joystick Settings")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;

    [Header("Visuals")]
    public Transform[] frontWheels; // Assign front wheels for steering rotation
    public Transform[] rearWheels;  // Assign rear wheels (optional, for spinning)
    public Transform bodyTransform; // Assign the truck's main body

    public float wheelTurnSpeed = 5f;
    public float wheelSpinSpeed = 360f; // Degrees per second when moving
    public float bodyTiltAmount = 10f;  // How much the body leans when turning
    public float bodyTiltSpeed = 5f;    // How fast the body tilts

    private Vector2 joystickInput;
    private Vector2 joystickCenter;
    private float joystickRadius;
    private bool isSprinting = false;

    private float currentSpeed = 0f;

    void Start()
    {
        joystickCenter = joystickBackground.position;
        joystickRadius = joystickBackground.sizeDelta.x / 2f;
    }

    void Update()
    {
        // Handle sprint input
        isSprinting = Input.GetKey(KeyCode.LeftShift) || isSprinting;

        float targetSpeed = maxSpeed * (isSprinting ? sprintMultiplier : 1f);
        float moveInput = joystickInput.y;  // Forward/backward
        float steerInput = joystickInput.x; // Left/right

        // Smoothly adjust speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, moveInput * targetSpeed, acceleration * Time.deltaTime);

        // Move the truck
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Rotate the truck (steering)
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float steer = steerInput * steeringAngle * Time.deltaTime;
            transform.Rotate(Vector3.up, steer);
        }

        // Reset joystick if not dragging
        if (!Input.GetMouseButton(0))
        {
            joystickInput = Vector2.zero;
            joystickHandle.position = joystickCenter;
        }

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

        HandleWheelVisuals(steerInput);
        HandleBodyTilt(steerInput, moveInput);
    }

    private void HandleWheelVisuals(float steerInput)
    {
        // Rotate front wheels for steering
        foreach (Transform wheel in frontWheels)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, steerInput * steeringAngle, 0f);
            wheel.localRotation = Quaternion.Lerp(wheel.localRotation, targetRotation, Time.deltaTime * wheelTurnSpeed);
        }

        // Spin all wheels when moving
        float spin = currentSpeed * wheelSpinSpeed * Time.deltaTime;
        foreach (Transform wheel in frontWheels)
        {
            wheel.Rotate(Vector3.right, spin);
        }
        foreach (Transform wheel in rearWheels)
        {
            wheel.Rotate(Vector3.right, spin);
        }
    }

    private void HandleBodyTilt(float steerInput, float moveInput)
    {
        if (bodyTransform == null) return;

        // Slight tilt based on turning (lean left/right) and acceleration (lean forward/back)
        float tiltZ = -steerInput * bodyTiltAmount; // Roll left/right
        float tiltX = -moveInput * (bodyTiltAmount * 0.5f); // Pitch forward/back

        Quaternion targetTilt = Quaternion.Euler(tiltX, 0f, tiltZ);
        bodyTransform.localRotation = Quaternion.Lerp(bodyTransform.localRotation, targetTilt, Time.deltaTime * bodyTiltSpeed);
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

    public void StartSprinting()
    {
        isSprinting = true;
    }

    public void StopSprinting()
    {
        isSprinting = false;
    }
}
