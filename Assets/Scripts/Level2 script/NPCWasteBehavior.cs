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
    public List<Transform> nearbyWaste;
    public Transform trashBin;
    public Transform carryPosition;

    private NavMeshAgent agent;
    private Transform currentWaste;

    private RubbishTracker rubbishTracker;
    private TrashBin trashBinComponent;

    private void Start()
    {
        currentResistance = maxResistance;
        agent = GetComponent<NavMeshAgent>();
        if (npcAnimator == null) npcAnimator = GetComponent<Animator>();
        rubbishTracker = FindObjectOfType<RubbishTracker>();

        if (trashBin != null)
        {
            trashBinComponent = trashBin.GetComponent<TrashBin>();
            if (trashBinComponent == null)
                Debug.LogWarning("Trash bin does not have a TrashBin component attached.");
        }
    }

    private void Update()
    {
        if (!isEducated && RubbishTracker.CorrectlyDisposedTotal >= 3)
        {
            npcAnimator.SetBool("ReadyToLearn", true);
        }
    }

    public void InteractWithPlayer()
    {
        if (isEducated || isSorting) return;

        if (currentResistance > 0)
        {
            currentResistance--;
            npcAnimator.SetTrigger("Stubborn");

            string[] responses = {
                "I don't think waste matters much...",
                "Really? Sorting trash is that important?",
                "Hmm... I'm starting to get it."
            };

            feedbackText.text = responses[maxResistance - currentResistance - 1] +
                                $" (Resistance left: {currentResistance})";
            Debug.Log(feedbackText.text);
        }
        else
        {
            EducateNPC();
        }
    }

    private void EducateNPC()
    {
        isEducated = true;
        feedbackText.text = "Thank you for explaining! I’ll start sorting waste!";
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
                feedbackText.text = "All nearby waste has been sorted!";
                npcAnimator.SetTrigger("Idle");
                break;
            }

            agent.SetDestination(currentWaste.position);
            while (Vector3.Distance(transform.position, currentWaste.position) > 1.2f)
                yield return null;

            npcAnimator.SetTrigger("Pick");
            yield return new WaitForSeconds(1f);

            currentWaste.SetParent(carryPosition);
            currentWaste.localPosition = Vector3.zero;
            currentWaste.localRotation = Quaternion.identity;

            agent.SetDestination(trashBin.position);
            while (Vector3.Distance(transform.position, trashBin.position) > 1.2f)
                yield return null;

            npcAnimator.SetTrigger("Sort");
            yield return new WaitForSeconds(2f);

            if (trashBinComponent != null)
            {
                if (!trashBinComponent.IsFull())
                {
                    HandleSuccessfulSort();
                }
                else
                {
                    TrashBin alternateBin = FindAvailableBin(trashBinComponent.GetCategory());

                    if (alternateBin != null)
                    {
                        trashBin = alternateBin.transform;
                        trashBinComponent = alternateBin;
                        feedbackText.text = "Bin was full. Redirecting to another bin...";

                        agent.SetDestination(trashBin.position);
                        while (Vector3.Distance(transform.position, trashBin.position) > 1.2f)
                            yield return null;

                        npcAnimator.SetTrigger("Sort");
                        yield return new WaitForSeconds(2f);

                        HandleSuccessfulSort();
                    }
                    else
                    {
                        HandleFailedSort();
                        break;
                    }
                }
            }
            else
            {
                feedbackText.text = "Missing trash bin logic.";
                break;
            }

            currentWaste = null;
            yield return new WaitForSeconds(1f);
        }

        isSorting = false;
    }

    private void HandleSuccessfulSort()
    {
        if (trashBinComponent.TryAddRubbish())
        {
            if (rubbishTracker != null)
                rubbishTracker.AddCorrectDisposal();

            feedbackText.text = "Waste sorted!";
            nearbyWaste.Remove(currentWaste);
            Destroy(currentWaste.gameObject);
        }
        else
        {
            feedbackText.text = "Unexpected error: Bin said it wasn’t full, but rejected the waste.";
        }
    }

    private void HandleFailedSort()
    {
        feedbackText.text = "No available bins. Waste dropped.";
        npcAnimator.SetTrigger("Fail");

        currentWaste.SetParent(null);
        nearbyWaste.Remove(currentWaste);
        currentWaste.position = transform.position + transform.forward * 1.2f;
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

    TrashBin FindAvailableBin(string category)
    {
        TrashBin[] allBins = GameObject.FindObjectsOfType<TrashBin>();
        foreach (TrashBin bin in allBins)
        {
            if (bin.GetCategory() == category && !bin.IsFull())
                return bin;
        }
        return null;
    }

    public bool IsEducated() => isEducated;
    public int GetCurrentResistance() => currentResistance;
}
