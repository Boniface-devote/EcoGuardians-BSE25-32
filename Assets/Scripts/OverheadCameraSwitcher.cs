using UnityEngine;

public class OverheadCameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera overheadCamera;
    public Transform player;
    public Vector3 overheadOffset = new Vector3(0f, 20f, 0f); // Height above player

    private bool isOverheadActive = false;

    void Start()
    {
        // Make sure the main camera is active initially
        if (mainCamera != null && overheadCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            overheadCamera.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        // If overhead view is active, follow player
        if (isOverheadActive && overheadCamera != null && player != null)
        {
            overheadCamera.transform.position = player.position + overheadOffset;
            overheadCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Look straight down
        }
    }

    // Called from UI button
    public void ToggleCameraView()
    {
        isOverheadActive = !isOverheadActive;

        if (mainCamera != null && overheadCamera != null)
        {
            mainCamera.gameObject.SetActive(!isOverheadActive);
            overheadCamera.gameObject.SetActive(isOverheadActive);
        }
    }
}
