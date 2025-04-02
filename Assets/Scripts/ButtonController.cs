using UnityEngine;
using TMPro;
using System.Diagnostics;

public class ButtonController : MonoBehaviour
{
    public TextMeshProUGUI feedbackText; // Reference to the feedback text
    private bool glovesOn = false; // Track gloves state
    private float feedbackDuration = 2f; // How long the feedback text is visible

    public void OnActionPress()
    {
        UnityEngine.Debug.Log("Action button pressed!");
        ShowFeedback("Action Performed!");
    }

    public void OnGlovesToggle()
    {
        glovesOn = !glovesOn;
        string message = glovesOn ? "Gloves On!" : "Gloves Off!";
        UnityEngine.Debug.Log(message);
        ShowFeedback(message);
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(true);
            feedbackText.text = message;
            Invoke(nameof(HideFeedback), feedbackDuration);
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.gameObject.SetActive(false);
            feedbackText.text = "";
        }
    }
}