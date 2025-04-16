using UnityEngine;

public class ShopperController : MonoBehaviour
{
    public float speed = 1f;              // Movement speed, adjustable in Inspector
    public Vector3 startPos;              // Start position, set in Inspector
    public Vector3 endPos;                // End position, set in Inspector
    public float idleDuration = 2f;       // Time to stay idle at each end, adjustable in Inspector
    public float rotationSpeed = 180f;    // Rotation speed in degrees per second, adjustable in Inspector

    private bool movingToEnd = true;      // Tracks direction
    private bool isIdle = false;          // Tracks idle state
    private float idleTimer = 0f;         // Timer for idle duration
    private Animator animator;            // For animations
    private Quaternion targetRotation;    // Target rotation to face movement direction

    void Start()
    {
        animator = GetComponent<Animator>();  // Get animator component
        if (startPos == Vector3.zero)         // Default to (-2, 1, 1) if not set
            startPos = new Vector3(-2, 1, 1);
        if (endPos == Vector3.zero)           // Default to (2, 1, 1) if not set
            endPos = new Vector3(2, 1, 1);
        transform.position = startPos;        // Start at assigned startPos

        // Initialize rotation to face endPos
        UpdateTargetRotation();
        transform.rotation = targetRotation;  // Set initial rotation immediately

        // Initialize animation state
        if (animator != null)
        {
            animator.SetBool("isWalking", true);  // Start with walking
        }
    }

    void Update()
    {
        if (isIdle)
        {
            // Rotate towards target rotation during idle
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Handle idle state
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                // Exit idle state
                isIdle = false;
                idleTimer = 0f;
                movingToEnd = !movingToEnd;  // Switch direction
                UpdateTargetRotation();    // Set rotation for next destination
                if (animator != null)
                {
                    animator.SetBool("isWalking", true);  // Start walking
                }
            }
            return;  // Skip movement while idle
        }

        // Move towards target (endPos or startPos)
        Vector3 target = movingToEnd ? endPos : startPos;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        // Keep facing movement direction while moving
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Check if reached target
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            // Snap to target to avoid floating-point issues
            transform.position = target;

            // Enter idle state
            isIdle = true;
            idleTimer = 0f;
            UpdateTargetRotation();  // Prepare to face opposite direction
            if (animator != null)
            {
                animator.SetBool("isWalking", false);  // Trigger idle animation
            }
        }
    }

    // Helper method to update target rotation based on movement direction
    private void UpdateTargetRotation()
    {
        Vector3 nextTarget = movingToEnd ? endPos : startPos;
        Vector3 direction = (nextTarget - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(direction);
        }
    }
}