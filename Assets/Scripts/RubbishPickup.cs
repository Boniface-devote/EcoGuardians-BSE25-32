using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RubbishPickup : MonoBehaviour
{
    public float interactionRange = 2f;
    public Transform holdPoint;
    public string trashBinTag = "TrashBin";
    public Text scoreText;

    [Header("Glove Settings")]
    public bool glovesOn = false;
    public Text glovesStatusText;
    public Text rubbishIndicator;

    private List<GameObject> carriedRubbish = new List<GameObject>();
    private string carriedCategory = null;
    private int score = 0;

    private Dictionary<string, string> rubbishCategories = new Dictionary<string, string>()
    {
        { "Battery", "Hazardous Waste" },
        { "MedicalWaste", "Hazardous Waste" },

        { "FoodScrap", "Organic Waste" },
        { "FoodWaste", "Organic Waste" },
        { "FruitWaste", "Organic Waste" },

        { "PlasticBottle", "Plastic Waste" },
        { "PlasticBag", "Plastic Waste" },

        { "Newspaper", "Paper Waste" },
        { "PackagingPaper", "Paper Waste" },

        { "GlassBottle", "Glass Waste" },
        { "BrokenGlass", "Glass Waste" },
        { "BeerBottle", "Glass Waste" },

        { "SodaCan", "Metal Waste" },
        { "MetalContainer", "Metal Waste" },
        { "ScrapMetal", "Metal Waste" },
       
    };

    void Start()
    {
        UpdateGlovesText();
    }

    void Update()
    {
        // Press "A" to pick up
        if (Input.GetKeyDown(KeyCode.A))
        {
            TryPickupRubbish();
        }

        // Press "X" to drop
        if (Input.GetKeyDown(KeyCode.X))
        {
            HandleDrop();
        }


        // Follow hold point
        for (int i = 0; i < carriedRubbish.Count; i++)
        {
            if (carriedRubbish[i] != null)
                carriedRubbish[i].transform.position = holdPoint.position + Vector3.up * (i * 0.3f); // stack them
        }
    }

    public void ToggleGloves()
    {
        glovesOn = !glovesOn;
        UpdateGlovesText();
    }

    private void UpdateGlovesText()
    {
        if (glovesStatusText != null)
        {
            glovesStatusText.text = "Gloves: " + (glovesOn ? "On" : "Off");
        }
    }

    public void TryPickupRubbish()
    {
        if (carriedRubbish.Count >= 3)
        {
            Debug.Log("You can't carry more than 3 items.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (Collider hit in hits)
        {
            string tag = hit.tag;

            if (rubbishCategories.ContainsKey(tag) && !carriedRubbish.Contains(hit.gameObject))
            {
                string category = rubbishCategories[tag];

                // Ensure same category
                if (carriedCategory == null || category == carriedCategory)
                {
                    GameObject rubbish = hit.gameObject;
                    carriedRubbish.Add(rubbish);
                    carriedCategory = category;

                    Rigidbody rb = rubbish.GetComponent<Rigidbody>();
                    if (rb != null) rb.isKinematic = true;

                    rubbish.transform.SetParent(holdPoint);

                    Debug.Log($"Picked up: {rubbish.name} | Category: {category}");
                    rubbishIndicator.text = $"Picked: {rubbish.name}| Category: {category}";

                    if ((category == "Hazardous Waste" || category == "Glass Waste") && !glovesOn)
                    {
                        Debug.LogWarning("Picked hazardous/sharp waste without gloves. Penalty applied.");
                        rubbishIndicator.text = "Picked hazardous/sharp waste without gloves. Penalty applied.";
                        score -= 1;
                        UpdateScoreUI();
                    }

                    return;
                }
                else
                {
                    Debug.Log("You can only carry rubbish of the same category.");
                    rubbishIndicator.text = "You can only carry rubbish of the same category.";
                    return;
                }
            }
        }

        Debug.Log("No valid rubbish nearby to pick up.");
        rubbishIndicator.text = "No valid rubbish nearby to pick up.";
    }

    public void HandleDrop()
    {
        if (carriedRubbish.Count == 0)
        {
            Debug.Log("You are not carrying any rubbish.");
            rubbishIndicator.text = "You are not carrying any rubbish.";
            return;
        }

        if (IsNearTrashBin())
        {
            DropAllInTrashBin();
        }
        else
        {
            DropAllRubbish();
        }
    }

    void DropAllRubbish()
    {
        foreach (GameObject rubbish in carriedRubbish)
        {
            if (rubbish != null)
            {
                rubbish.transform.SetParent(null);

                Rigidbody rb = rubbish.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = false;
            }
        }

        Debug.Log("Dropped all rubbish on the ground.");
        rubbishIndicator.text = "Dropped all rubbish on the ground.";
        carriedRubbish.Clear();
        carriedCategory = null;
    }

    void DropAllInTrashBin()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(trashBinTag))
            {
                TrashBin bin = hit.GetComponent<TrashBin>();
                if (bin == null)
                    return;

                foreach (GameObject rubbish in new List<GameObject>(carriedRubbish))
                {
                    if (bin.IsFull())
                    {
                        Debug.LogWarning("Bin is full! Cannot drop more rubbish.");
                        rubbishIndicator.text = "Bin is full! Cannot drop more rubbish.";
                        break;
                    }

                    string tag = rubbish.tag;
                    string category = rubbishCategories.ContainsKey(tag) ? rubbishCategories[tag] : "Unknown";

                    if (category == bin.GetCategory())
                    {
                        if (bin.TryAddRubbish())
                        {
                            Debug.Log($"Correct disposal: {rubbish.name} into {category} bin.");
                            rubbishIndicator.text = $"Correct disposal: {rubbish.name} into {category} bin.";
                            score += 1;
                            FindObjectOfType<RubbishTracker>().AddCorrectDisposal();

                        }
                    }
                    else
                    {
                        if (bin.TryAddRubbish())
                        {
                            Debug.LogWarning($"Incorrect disposal: {rubbish.name} is {category}, bin is {bin.GetCategory()}.");
                            rubbishIndicator.text = $"Incorrect disposal: {rubbish.name} is {category}, bin is {bin.GetCategory()}.";
                            score -= 1;
                        }
                    }

                    Destroy(rubbish);
                    carriedRubbish.Remove(rubbish);
                }

                carriedCategory = null;
                UpdateScoreUI();
                return;
            }
        }

        Debug.Log("No valid trash bin nearby.");
        rubbishIndicator.text = "No valid trash bin nearby.";
    }

    bool IsNearTrashBin()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(trashBinTag))
                return true;
        }
        return false;
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
