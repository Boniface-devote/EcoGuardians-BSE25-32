using UnityEngine;

public class JoystickAnimatorController : MonoBehaviour
{
    public Animator playerAnimator; // Reference to the Animator component
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float movementThreshold = 0.1f; // Minimum movement to consider "active"

    private Vector2 joystickCenter;
    private float joystickRadius;
    private Vector2 joystickInput;

    void Start()
    {
        if (joystickBackground == null || joystickHandle == null)
        {
            Debug.LogError("Joystick references are not assigned!");
            enabled = false;
            return;
        }

        joystickCenter = joystickBackground.position;
        joystickRadius = joystickBackground.sizeDelta.x / 2f;
    }

    void Update()
    {
        // Check if the user is touching the screen
        if (Input.GetMouseButton(0))
        {
            Vector2 screenPosition = Input.mousePosition;
            Vector2 localPoint;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBackground, screenPosition, null, out localPoint);

            localPoint = Vector2.ClampMagnitude(localPoint, joystickRadius);
            joystickInput = localPoint / joystickRadius;
        }
        else
        {
            joystickInput = Vector2.zero;
        }

        // Enable/disable the Animator based on movement
        if (playerAnimator != null)
        {
            playerAnimator.enabled = joystickInput.magnitude > movementThreshold;
        }

        // Optional: Reset joystick handle position visually
        if (!Input.GetMouseButton(0))
        {
            joystickHandle.position = joystickCenter;
        }
    }
}
