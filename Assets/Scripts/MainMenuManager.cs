using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Optional Audio")]
    public AudioSource clickSound;

    // Load scene by name
    public void LoadLevel1(string sceneName)
    {
        PlayClickSound();
        SceneManager.LoadScene("Level1");
    }

    // Quit the game
    public void QuitGame()
    {
        PlayClickSound();
        Debug.Log("Quit Game!");
        Application.Quit();
    }

    // Optional: Plays UI click sound
    private void PlayClickSound()
    {
        if (clickSound != null)
            clickSound.Play();
    }
}
