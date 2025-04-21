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

    private float timer;
    public int totalRubbishCount;
    public int correctlyDisposedCount = 0;

    public List<GameObject> allRubbish = new List<GameObject>();
    private bool stageEnded = false;

    void Start()
    {
        timer = stageTimeLimit;

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
        totalRubbish.text = "Total rubbish:" + totalRubbishCount.ToString();
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
        correctlyDisposals.text="Correct Disposals: "+correctlyDisposedCount.ToString();
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
                resultText.text = " Not enough rubbish disposed! Try again.";

            Debug.LogWarning("Stage Failed! Less than 80% cleared.");
            Invoke("RestartStage", 3f); // restart after short delay
        }
    }

    void StageCompleted()
    {
        stageEnded = true;
        if (resultText != null)
            resultText.text = " Stage Complete! Good job!";
        Debug.Log("Stage completed successfully!");
        // You can add level transition here
    }

    void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
