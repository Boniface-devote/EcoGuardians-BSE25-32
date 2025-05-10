using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UsernameCapture : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public Button joinButton;
    public GameObject mainMenuPanel; // Reference to the main menu panel
    public GameObject usernamePanel; // Reference to the username input panel

    private const string UsernameKey = "Username";

    private void Start()
    {
        // Check if user is already registered
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            // Load main menu directly
            ShowMainMenu();
        }
        else
        {
            // Show username input panel
            usernamePanel.SetActive(true);
            joinButton.onClick.AddListener(SaveUsername);

            // Optional: Pre-fill with saved username if exists
            if (PlayerPrefs.HasKey(UsernameKey))
            {
                usernameInputField.text = PlayerPrefs.GetString(UsernameKey);
            }
        }
    }

    void SaveUsername()
    {
        string username = usernameInputField.text.Trim();

        if (!string.IsNullOrEmpty(username))
        {
            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.Save();
            Debug.Log("Username saved: " + username);
            ShowMainMenu();
        }
        else
        {
            Debug.LogWarning("Username field is empty!");
        }
    }

    void ShowMainMenu()
    {
        usernamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}