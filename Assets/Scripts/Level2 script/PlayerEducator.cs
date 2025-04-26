using UnityEngine;
using UnityEngine.UI;

public class PlayerEducator : MonoBehaviour
{
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public Text interactionPrompt;
    public Button interactButton;

    private NPCWasteBehavior closestStandardNPC;
    private BlackNPCWasteBehavior closestBlackNPC;

    void Start()
    {
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnInteractButtonPressed);
            interactButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (RubbishTracker.CorrectlyDisposedTotal < 3)
        {
            interactionPrompt.text = "Sort 3 items to gain trust!";
            interactButton?.gameObject.SetActive(false);
            return;
        }

        FindClosestNPCs();
        UpdateInteractionPrompt();

        if (Input.GetKeyDown(interactKey))
        {
            InteractWithNPC();
        }
    }

    void OnInteractButtonPressed()
    {
        InteractWithNPC();
    }

    void InteractWithNPC()
    {
        if (closestBlackNPC != null)
            closestBlackNPC.InteractWithPlayer();
        else if (closestStandardNPC != null)
            closestStandardNPC.InteractWithPlayer();
    }

    void UpdateInteractionPrompt()
    {
        if (closestBlackNPC != null && !closestBlackNPC.IsEducated())
        {
            interactionPrompt.text = closestBlackNPC.HasDisposedIncorrectly()
                ? "They threw it in the wrong bin! Let's explain again."
                : "Press [E] or Tap to talk about sorting waste.";

            interactButton?.gameObject.SetActive(true);
        }
        else if (closestStandardNPC != null && !closestStandardNPC.IsEducated())
        {
            int resistanceLeft = closestStandardNPC.GetCurrentResistance();

            string playerMessage = resistanceLeft switch
            {
                3 => "Hello! Do you know why sorting trash is important?",
                2 => "Waste can be recycled if sorted.",
                1 => "Yes! Sorting waste leads to a clean environment.",
                0 => "Thanks for listening! You got it now!",
                _ => "Press [E] or tap the button to talk about waste management."
            };

            interactionPrompt.text = playerMessage;
            interactButton?.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt.text = "";
            interactButton?.gameObject.SetActive(false);
        }
    }

    void FindClosestNPCs()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);
        float minDistStandard = Mathf.Infinity;
        float minDistBlack = Mathf.Infinity;
        closestStandardNPC = null;
        closestBlackNPC = null;

        foreach (Collider hit in hits)
        {
            var blackNPC = hit.GetComponent<BlackNPCWasteBehavior>();
            if (blackNPC != null && !blackNPC.IsEducated())
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistBlack)
                {
                    minDistBlack = dist;
                    closestBlackNPC = blackNPC;
                }
                continue;
            }

            var standardNPC = hit.GetComponent<NPCWasteBehavior>();
            if (standardNPC != null && !standardNPC.IsEducated())
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDistStandard)
                {
                    minDistStandard = dist;
                    closestStandardNPC = standardNPC;
                }
            }
        }
    }
}
