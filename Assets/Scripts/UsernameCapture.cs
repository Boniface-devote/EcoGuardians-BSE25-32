using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UsernameCapture : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public Button joinButton;

    private const string UsernameKey = "Username";

    private void Start()
    {
        // Optional: Load previously saved username
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            usernameInputField.text = PlayerPrefs.GetString(UsernameKey);
        }

        joinButton.onClick.AddListener(SaveUsername);
    }

    void SaveUsername()
    {
        string username = usernameInputField.text.Trim();

        if (!string.IsNullOrEmpty(username))
        {
            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.Save();
            Debug.Log("Username saved: " + username);
        }
        else
        {
            Debug.LogWarning("Username field is empty!");
        }
    }
}
