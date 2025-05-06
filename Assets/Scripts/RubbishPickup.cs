using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    public bool demoCompleted = false;
    private Coroutine messageCoroutine;

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
        rubbishIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TryPickupRubbish();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            HandleDrop();
        }

        for (int i = 0; i < carriedRubbish.Count; i++)
        {
            if (carriedRubbish[i] != null)
                carriedRubbish[i].transform.position = holdPoint.position + Vector3.up * (i * 0.3f);
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
            ShowMessage("You can't carry more than 3 items.", true);
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (Collider hit in hits)
        {
            string tag = hit.tag;

            if (rubbishCategories.ContainsKey(tag) && !carriedRubbish.Contains(hit.gameObject))
            {
                string category = rubbishCategories[tag];

                if (carriedCategory == null || category == carriedCategory)
                {
                    GameObject rubbish = hit.gameObject;
                    carriedRubbish.Add(rubbish);
                    carriedCategory = category;

                    Rigidbody rb = rubbish.GetComponent<Rigidbody>();
                    if (rb != null) rb.isKinematic = true;

                    rubbish.transform.SetParent(holdPoint);

                    ShowMessage($"Picked: {rubbish.name}| Category: {category}", false);

                    if ((category == "Hazardous Waste" || category == "Glass Waste") && !glovesOn)
                    {
                        score -= 1;
                        UpdateScoreUI();
                        ShowMessage("Picked hazardous/sharp waste without gloves. Penalty applied.", true);
                    }

                    return;
                }
                else
                {
                    ShowMessage("You can only carry rubbish of the same category.", true);
                    return;
                }
            }
        }

        ShowMessage("No valid rubbish nearby to pick up.", true);
    }

    public void HandleDrop()
    {
        if (carriedRubbish.Count == 0)
        {
            ShowMessage("You are not carrying any rubbish.", true);
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

        ShowMessage("Dropped all rubbish on the ground.", false);
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
                        ShowMessage("Bin is full! Cannot drop more rubbish.", true);
                        break;
                    }

                    string tag = rubbish.tag;
                    string category = rubbishCategories.ContainsKey(tag) ? rubbishCategories[tag] : "Unknown";

                    if (category == bin.GetCategory())
                    {
                        if (bin.TryAddRubbish())
                        {
                            score += 1;
                            demoCompleted = true;
                            FindObjectOfType<RubbishTracker>().AddCorrectDisposal();
                            FindObjectOfType<CategoryRubbishTracker>()?.RegisterRubbish(category, true);

                            ShowMessage($"Correct disposal: {rubbish.name} into {category} bin.", false);
                        }
                    }
                    else
                    {
                        if (bin.TryAddRubbish())
                        {
                            score -= 1;
                            FindObjectOfType<CategoryRubbishTracker>()?.RegisterRubbish(category, false);

                            ShowMessage($"Incorrect disposal: {rubbish.name} is {category}, bin is {bin.GetCategory()}.", true);
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

        ShowMessage("No valid trash bin nearby.", true);
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

    void ShowMessage(string message, bool isWarning)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);
        messageCoroutine = StartCoroutine(DisplayMessage(message, isWarning));
    }

    IEnumerator DisplayMessage(string message, bool isWarning)
    {
        rubbishIndicator.text = message;
        rubbishIndicator.color = isWarning ? Color.red : Color.green;
        rubbishIndicator.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        rubbishIndicator.gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
