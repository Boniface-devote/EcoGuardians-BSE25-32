using UnityEngine;

public class OverheadCameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera overheadCamera;
    public Camera carCamera;
    public Transform player;
    public Transform car;
    public Vector3 overheadOffset = new Vector3(0f, 20f, 0f);

    public float interactionDistance = 3f;
    public float exitDistanceFromCar = 4f; // Distance to place player when exiting

    public GameObject steeringUICanvas;  // Drag your Steering UI Canvas here
    public GameObject joystickUICanvas;  // Drag your Joystick UI Canvas here

    private bool isOverheadActive = false;
    private bool isInCar = false;
    private BoxCollider playerCollider;

    void Start()
    {
        if (mainCamera != null && overheadCamera != null && carCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            overheadCamera.gameObject.SetActive(false);
            carCamera.gameObject.SetActive(false);
        }

        if (player != null)
        {
            playerCollider = player.GetComponent<BoxCollider>();
            if (playerCollider == null)
            {
                Debug.LogWarning("Player does not have a BoxCollider component!");
            }
        }

        UpdateUIState();
    }

    void Update()
    {
        if (player == null || car == null)
            return;

        if (!isInCar)
        {
            float distanceToCar = Vector3.Distance(player.position, car.position);

            if (distanceToCar <= interactionDistance && Input.GetKeyDown(KeyCode.E))
            {
                EnterCar();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitCar();
            }
        }
    }

    void LateUpdate()
    {
        if (isOverheadActive && overheadCamera != null && player != null && !isInCar)
        {
            overheadCamera.transform.position = player.position + overheadOffset;
            overheadCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    public void ToggleCameraView()
    {
        if (isInCar) return; // Don't allow overhead view while inside the car

        isOverheadActive = !isOverheadActive;

        if (mainCamera != null && overheadCamera != null)
        {
            mainCamera.gameObject.SetActive(!isOverheadActive);
            overheadCamera.gameObject.SetActive(isOverheadActive);
        }
    }

    void EnterCar()
    {
        isInCar = true;

        mainCamera.gameObject.SetActive(false);
        overheadCamera.gameObject.SetActive(false);

        if (carCamera != null)
        {
            carCamera.gameObject.SetActive(true);
        }

        // 🔥 Disable player object
        if (player != null)
        {
            player.gameObject.SetActive(false);
        }

        UpdateUIState();
    }

    void ExitCar()
    {
        isInCar = false;

        if (carCamera != null)
        {
            carCamera.gameObject.SetActive(false);
        }

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }

        // 🔥 Enable player object
        if (player != null)
        {
            player.gameObject.SetActive(true);
        }

        // Calculate correct respawn position
        Vector3 exitDirection = car.right; // Exit to the right of the car
        Vector3 offset = exitDirection * (exitDistanceFromCar + GetPlayerHalfWidth());
        Vector3 newPosition = car.position + offset;

        // Force Y to be exactly 1
        newPosition.y = 1f;

        player.position = newPosition;
        player.SetParent(null);

        UpdateUIState();
    }

    float GetPlayerHalfWidth()
    {
        if (playerCollider != null)
        {
            return playerCollider.size.x * 0.5f * player.localScale.x;
        }
        return 0f;
    }

    void UpdateUIState()
    {
        if (steeringUICanvas != null)
            steeringUICanvas.SetActive(isInCar);

        if (joystickUICanvas != null)
            joystickUICanvas.SetActive(!isInCar);
    }

    public void OnEnterExitButtonPressed()
    {
        if (!isInCar)
        {
            float distanceToCar = Vector3.Distance(player.position, car.position);
            if (distanceToCar <= interactionDistance)
            {
                EnterCar();
            }
        }
        else
        {
            ExitCar();
        }
    }
}
