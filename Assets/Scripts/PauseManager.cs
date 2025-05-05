using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenu;

    private bool isPaused = false;

    void Update()
    {
        // Toggle pause on 'P' key
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        // Exit to main menu on 'Escape' key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitToMainMenu();
        }
    }

    // Toggle pause state
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenu != null)
                pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenu != null)
                pauseMenu.SetActive(false);
        }
    }

    // Exit to main menu
    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time is resumed
        SceneManager.LoadScene("MainMenu"); // Adjust name to your actual main menu scene
    }
}
