using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Optional Audio")]
    public AudioSource clickSound;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject aboutPanel;
    public GameObject levelPanel;
    public GameObject Registration;

    // Load scene by name
    public void LoadLevel1(string sceneName)
    {
        PlayClickSound();
        SceneManager.LoadScene("Level1");
    }
    public void LoadLevel2(string sceneName)
    {
        PlayClickSound();
        SceneManager.LoadScene("Level2");
    }

    // Quit the game
    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Quit Game!");
        Application.Quit();
    }
    // Switches between two specified panels
    public void AboutPanel()
    {
        PlayClickSound();

        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel to hide is not assigned in the Inspector.");
        }

        if (aboutPanel != null)
        {
            aboutPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel to show is not assigned in the Inspector.");
        }
    }
    public void LevelPanel()
    {
        PlayClickSound();

        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel to hide is not assigned in the Inspector.");
        }

        if (aboutPanel != null)
        {
            levelPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel to show is not assigned in the Inspector.");
        }
    }
    public void Back()
    {
        PlayClickSound();

        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel to hide is not assigned in the Inspector.");
        }

        if (aboutPanel != null)
        {
            aboutPanel.SetActive(false);
            levelPanel.SetActive(false);
            Registration.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel to show is not assigned in the Inspector.");
        }
    }
    // Optional: Plays UI click sound
    private void PlayClickSound()
    {
        if (clickSound != null)
            clickSound.Play();
    }
}
