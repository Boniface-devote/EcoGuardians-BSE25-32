using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public string binCategory; // e.g. "Hazardous Waste", "Organic Waste"
    public int maxCapacity = 2; // Maximum number of items this bin can hold
    private int currentCount = 0;

    private Renderer binRenderer;

    void Start()
    {
        binRenderer = GetComponent<Renderer>();
        AssignColorByCategory();
    }

    // Assigns a unique color to the bin based on its category
    void AssignColorByCategory()
    {
        if (binRenderer == null) return;

        Color binColor;

        switch (binCategory)
        {
            case "Hazardous Waste":
                binColor = Color.red;
                break;
            case "Organic Waste":
                binColor = new Color(0.4f, 0.8f, 0.4f); // greenish
                break;
            case "Plastic Waste":
                binColor = Color.yellow;
                break;
            case "Paper Waste":
                binColor = Color.white;
                break;
            case "Glass Waste":
                binColor = Color.blue;
                break;
            case "Metal Waste":
                binColor = Color.gray;
                break;
            default:
                binColor = Color.black;
                break;
        }

        binRenderer.material.color = binColor;
    }

    public string GetCategory()
    {
        return binCategory;
    }

    // Call this when rubbish is correctly dropped in
    public bool TryAddRubbish()
    {
        if (currentCount >= maxCapacity)
        {
            Debug.LogWarning($"Bin for {binCategory} is full!");
            return false;
        }

        currentCount++;
        return true;
    }

    public bool IsFull()
    {
        return currentCount >= maxCapacity;
    }
}
