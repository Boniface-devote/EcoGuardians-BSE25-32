using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RubbishTracker : MonoBehaviour
{
    public float stageTimeLimit = 300f; // 5 minutes in seconds
    public Text timerText;
    public Text resultText;
    public Text totalRubbish;
    public Text correctlyDisposals;
    public Button retryButton; // ✅ Add retry button

    private float timer;
    public int totalRubbishCount;
    public int correctlyDisposedCount = 0;

    public List<GameObject> allRubbish = new List<GameObject>();
    private bool stageEnded = false;

    void Start()
    {
        timer = stageTimeLimit;

        // Disable retry button initially
        if (retryButton != null)
            retryButton.gameObject.SetActive(false);

        // Find all rubbish objects in the scene
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Battery") || obj.CompareTag("FoodScrap") || obj.CompareTag("PlasticBottle") ||
                obj.CompareTag("GlassBottle") || obj.CompareTag("SodaCan") || obj.CompareTag("Newspaper") ||
                obj.CompareTag("PackagingPaper") ||
                obj.CompareTag("ScrapMetal") || obj.CompareTag("BeerBottle") ||
                obj.CompareTag("BrokenGlass") || obj.CompareTag("PlasticBag") ||
                obj.CompareTag("FruitWaste") || obj.CompareTag("MedicalWaste") || obj.CompareTag("MetalContainer"))
            {
                allRubbish.Add(obj);
            }
        }

        totalRubbishCount = allRubbish.Count;
        Debug.Log("Total Rubbish Found: " + totalRubbishCount);
        totalRubbish.text = "Total rubbish: " + totalRubbishCount.ToString();

        // Assign retry action if button exists
        if (retryButton != null)
            retryButton.onClick.AddListener(RetryStage);
    }

    void Update()
    {
        if (stageEnded) return;

        // Countdown timer
        timer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = "Time Left: " + Mathf.CeilToInt(timer) + "s";

        if (timer <= 0)
        {
            EndStage();
        }
    }

    public void AddCorrectDisposal()
    {
        correctlyDisposedCount++;
        correctlyDisposals.text = "Correct Disposals: " + correctlyDisposedCount.ToString();
        float progress = (float)correctlyDisposedCount / totalRubbishCount;
        if (progress >= 0.8f && !stageEnded)
        {
            StageCompleted();
        }
    }

    void EndStage()
    {
        stageEnded = true;

        float progress = (float)correctlyDisposedCount / totalRubbishCount;

        if (progress >= 0.8f)
        {
            StageCompleted();
        }
        else
        {
            if (resultText != null)
                resultText.text = "Not enough rubbish disposed! Try again.";

            Debug.LogWarning("Stage Failed! Less than 80% cleared.");

            // ✅ Show Retry Button
            if (retryButton != null)
                retryButton.gameObject.SetActive(true);
        }
    }

    void StageCompleted()
    {
        stageEnded = true;
        if (resultText != null)
            resultText.text = "Stage Complete! Good job!";
        Debug.Log("Stage completed successfully!");
    }

    // ✅ Called by retry button
    public void RetryStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
