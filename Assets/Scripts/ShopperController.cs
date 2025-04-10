using UnityEngine;

public class ShopperController : MonoBehaviour
{
    public float speed = 1f;              // Movement speed, adjustable in Inspector
    public Vector3 startPos;              // Start position, set in Inspector
    public Vector3 endPos;                // End position, set in Inspector
    private bool movingToEnd = true;      // Tracks direction
    private Animator animator;            // For walking animation
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();  // Get animator if present
        if (startPos == Vector3.zero)         // Default to (-2, 1, 1) if not set
            startPos = new Vector3(-2, 1, 1);
        if (endPos == Vector3.zero)           // Default to (2, 1, 1) if not set
            endPos = new Vector3(2, 1, 1);
        transform.position = startPos;        // Start at assigned startPos   
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = movingToEnd ? endPos : startPos;
        transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

        // Play walking animation if available
        if (animator != null && animator.HasState(0, Animator.StringToHash("Walk")))
        {
            animator.Play("Walk");
        }

        // Switch direction when close to target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingToEnd = !movingToEnd;
        }
    }
}

