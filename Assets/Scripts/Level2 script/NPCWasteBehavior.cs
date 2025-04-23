using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPCWasteBehavior : MonoBehaviour
{
    public int maxResistance = 3;
    private int currentResistance;
    private bool isEducated = false;
    private bool isSorting = false;

    public Animator npcAnimator;
    public Text feedbackText;
    public List<Transform> nearbyWaste; // Updated: list of waste items
    public Transform trashBin;          // Assign trash bin
    public Transform carryPosition;     // NPC's hand or carry position

    private NavMeshAgent agent;
    private Transform currentWaste;

    // ✅ Reference to RubbishTracker
    private RubbishTracker rubbishTracker;

    private void Start()
    {
        currentResistance = maxResistance;
        agent = GetComponent<NavMeshAgent>();
        if (npcAnimator == null) npcAnimator = GetComponent<Animator>();

        // ✅ Find the RubbishTracker in the scene
        rubbishTracker = FindObjectOfType<RubbishTracker>();
        if (rubbishTracker == null)
            Debug.LogWarning("RubbishTracker not found in scene!");
    }

    public void InteractWithPlayer()
    {
        if (isEducated || isSorting) return;

        if (currentResistance > 0)
        {
            currentResistance--;
            feedbackText.text = "NPC learning, resistance remaining: " + currentResistance;
            npcAnimator.SetTrigger("Stubborn");
        }
        else
        {
            EducateNPC();
        }
    }

    private void EducateNPC()
    {
        isEducated = true;
        feedbackText.text = "NPC educated! Starting to sort waste...";
        npcAnimator.SetTrigger("Educated");

        StartCoroutine(WasteSortingLoop());
    }

    IEnumerator WasteSortingLoop()
    {
        isSorting = true;

        while (true)
        {
            currentWaste = FindNearestWaste();
            if (currentWaste == null)
            {
                feedbackText.text = "No more waste to sort.";
                npcAnimator.SetTrigger("Idle");
                break;
            }

            // Move to waste
            agent.SetDestination(currentWaste.position);
            while (Vector3.Distance(transform.position, currentWaste.position) > 1.2f)
                yield return null;

            npcAnimator.SetTrigger("Pick");
            feedbackText.text = "Picked up waste. Heading to trash bin...";
            yield return new WaitForSeconds(1f);

            // Attach waste to hand
            currentWaste.SetParent(carryPosition);
            currentWaste.localPosition = Vector3.zero;
            currentWaste.localRotation = Quaternion.identity;

            // Move to bin
            agent.SetDestination(trashBin.position);
            while (Vector3.Distance(transform.position, trashBin.position) > 1.2f)
                yield return null;

            npcAnimator.SetTrigger("Sort");
            yield return new WaitForSeconds(2f);

            // ✅ Track correct disposal
            if (rubbishTracker != null)
                rubbishTracker.AddCorrectDisposal();

            // Dispose waste
            feedbackText.text = "NPC disposed the waste!";
            nearbyWaste.Remove(currentWaste);
            Destroy(currentWaste.gameObject);
            currentWaste = null;

            yield return new WaitForSeconds(1f);
        }

        isSorting = false;
    }

    Transform FindNearestWaste()
    {
        float shortestDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (Transform waste in nearbyWaste)
        {
            if (waste != null)
            {
                float dist = Vector3.Distance(transform.position, waste.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    nearest = waste;
                }
            }
        }

        return nearest;
    }

    public bool IsEducated() => isEducated;
}
