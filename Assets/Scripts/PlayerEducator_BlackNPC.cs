using UnityEngine;
using UnityEngine.UI;

public class PlayerEducator_BlackNPC : MonoBehaviour
{
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;
    public Text interactionPrompt;
    public Button interactButton;

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
        closestBlackNPC = FindClosestBlackNPC();
        UpdateInteractionPrompt();

        if (Input.GetKeyDown(interactKey) && closestBlackNPC != null)
        {
            closestBlackNPC.InteractWithPlayer();
        }
    }

    void OnInteractButtonPressed()
    {
        if (closestBlackNPC != null)
        {
            closestBlackNPC.InteractWithPlayer();
        }
    }

    void UpdateInteractionPrompt()
    {
        if (closestBlackNPC == null || closestBlackNPC.IsEducated())
        {
            interactionPrompt.text = "";
            if (interactButton != null) interactButton.gameObject.SetActive(false);
            return;
        }

        if (!closestBlackNPC.HasDisposedIncorrectly())
        {
            interactionPrompt.text = "Press [E] or Tap to talk about sorting waste.";
        }
        else
        {
            interactionPrompt.text = "They threw it in the wrong bin! Let's explain again.";
        }

        if (interactButton != null) interactButton.gameObject.SetActive(true);
    }

    BlackNPCWasteBehavior FindClosestBlackNPC()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);
        float minDist = Mathf.Infinity;
        BlackNPCWasteBehavior closest = null;

        foreach (Collider hit in hits)
        {
            BlackNPCWasteBehavior blackNPC = hit.GetComponent<BlackNPCWasteBehavior>();
            if (blackNPC != null && !blackNPC.IsEducated())
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = blackNPC;
                }
            }
        }

        return closest;
    }
}
