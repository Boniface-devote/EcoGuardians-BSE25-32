using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RubbishTracker : MonoBehaviour
{
    public float stageTimeLimit = 10f; // 5 minutes in seconds
    public Text timerText;
    public Text resultText;
    public Text totalRubbish;
    public Text totalRubbishForStageResults;
    public Text correctlyDisposals;

    public GameObject retryPanel; // ✅ Retry Panel instead of Button
    public GameObject ControllerPanel; // ✅ Retry Panel instead of Button
    public GameObject ecoWarriorBadgeIcon;   // Badge for Level1
    public GameObject ecoGuardianBadgeIcon;  // Badge for Level2

    private float timer;
    public int totalRubbishCount;
    public int correctlyDisposedCount = 0;

    public List<GameObject> allRubbish = new List<GameObject>();
    private bool stageEnded = false;

    void Start()
    {
        timer = stageTimeLimit;

        // Disable retry panel initially
        if (retryPanel != null)
            retryPanel.SetActive(false);

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
        totalRubbishForStageResults.text = "Total rubbish: " + totalRubbishCount.ToString();

        // Assign retry action from panel's retry button
        if (retryPanel != null)
        {
            Button retryButton = retryPanel.GetComponentInChildren<Button>();
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryStage);
        }
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

    public static int CorrectlyDisposedTotal { get; private set; }

    public void AddCorrectDisposal()
    {
        correctlyDisposedCount++;
        CorrectlyDisposedTotal = correctlyDisposedCount;

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

            // ✅ Show Retry Panel
            if (retryPanel != null)
                retryPanel.SetActive(true);
                ControllerPanel.SetActive(false);
        }
    }

    void StageCompleted()
    {
        stageEnded = true;
        if (resultText != null)
            resultText.text = "Stage Complete! Good job!";

        // Show badge based on scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Level1" && ecoWarriorBadgeIcon != null)
        {
            ecoWarriorBadgeIcon.SetActive(true);
        }
        else if (currentScene == "Level2" && ecoGuardianBadgeIcon != null)
        {
            ecoGuardianBadgeIcon.SetActive(true);
        }

        Debug.Log("Stage completed successfully!");

        StartCoroutine(LoadNextSceneAfterDelay());
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("mainMenu");
    }

    // ✅ Called by retry panel button
    public void RetryStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
