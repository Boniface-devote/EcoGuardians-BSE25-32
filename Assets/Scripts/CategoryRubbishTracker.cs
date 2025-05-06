using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CategoryRubbishTracker : MonoBehaviour
{
    [Header("UI Elements")]
    public Text plasticsText;
    public Text organicText;
    public Text paperText;
    public Text metalText;
    public Text glassText;
    public Text hazardousText;
    public Text totalRubbishText;
    public Text correctDisposalsText;

    private Dictionary<string, int> correctByCategory = new Dictionary<string, int>()
    {
        { "Plastic Waste", 0 },
        { "Organic Waste", 0 },
        { "Paper Waste", 0 },
        { "Metal Waste", 0 },
        { "Glass Waste", 0 },
        { "Hazardous Waste", 0 }
    };

    private int totalCorrectDisposals = 0;
    private int totalRubbishCount = 0;

    public void RegisterRubbish(string category, bool wasCorrect)
    {
        totalRubbishCount++;

        if (wasCorrect && correctByCategory.ContainsKey(category))
        {
            correctByCategory[category]++;
            totalCorrectDisposals++;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (plasticsText) plasticsText.text = "Plastics: " + correctByCategory["Plastic Waste"];
        if (organicText) organicText.text = "Organic: " + correctByCategory["Organic Waste"];
        if (paperText) paperText.text = "Paper: " + correctByCategory["Paper Waste"];
        if (metalText) metalText.text = "Metal: " + correctByCategory["Metal Waste"];
        if (glassText) glassText.text = "Glass: " + correctByCategory["Glass Waste"];
        if (hazardousText) hazardousText.text = "Hazardous: " + correctByCategory["Hazardous Waste"];

        if (totalRubbishText) totalRubbishText.text = "Total Rubbish: " + totalRubbishCount;
        if (correctDisposalsText) correctDisposalsText.text = "Correct Disposal: " + totalCorrectDisposals;
    }
}
