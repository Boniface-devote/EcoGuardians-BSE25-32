using UnityEngine;
using UnityEngine.UI;

public class UsernameDisplay : MonoBehaviour
{
    public Text displayText;
    private const string UsernameKey = "Username";

    void Start()
    {
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            string username = PlayerPrefs.GetString(UsernameKey);
            displayText.text =  username + "!";
        }
        else
        {
            displayText.text = "Warrior!";
        }
    }
}
