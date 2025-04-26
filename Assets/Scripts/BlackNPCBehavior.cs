using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BlackNPCWasteBehavior : MonoBehaviour
{
    public Animator npcAnimator;
    public Text feedbackText;
    public Transform incorrectTrashBin;
    public Transform correctTrashBin;
    public List<Transform> nearbyWaste;
    public Transform carryPosition;

    private NavMeshAgent agent;
    private Transform currentWaste;
    private TrashBin incorrectBinComponent;
    private TrashBin correctBinComponent;
    private RubbishTracker rubbishTracker;

    private int resistance = 1;
    private bool isEducated = false;
    private bool hasDisposedIncorrectly = false;
    private bool isSorting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (npcAnimator == null) npcAnimator = GetComponent<Animator>();
        rubbishTracker = FindObjectOfType<RubbishTracker>();

        if (incorrectTrashBin != null)
            incorrectBinComponent = incorrectTrashBin.GetComponent<TrashBin>();
        if (correctTrashBin != null)
            correctBinComponent = correctTrashBin.GetComponent<TrashBin>();
    }

    void Update()
    {
        // Idle animation can be handled here if needed
    }

    public void InteractWithPlayer()
    {
        if (isSorting || isEducated) return;

        if (resistance > 0)
        {
            resistance--;
            npcAnimator.SetTrigger("Stubborn");
            feedbackText.text = "Huh? This bin looks fine to me...";
            StartCoroutine(SortWasteIncorrectly());
        }
        else
        {
            EducateAndCorrect();
        }
    }

    void EducateAndCorrect()
    {
        isEducated = true;
        npcAnimator.SetTrigger("Educated");
        feedbackText.text = "Oh! I see now. Let me fix that.";

        StartCoroutine(SortWasteCorrectly());
    }

    IEnumerator SortWasteIncorrectly()
    {
        isSorting = true;
        currentWaste = FindNearestWaste();
        if (currentWaste == null)
        {
            feedbackText.text = "No waste nearby.";
            isSorting = false;
            yield break;
        }

        agent.SetDestination(currentWaste.position);
        while (Vector3.Distance(transform.position, currentWaste.position) > 1.2f)
            yield return null;

        npcAnimator.SetTrigger("Pick");
        yield return new WaitForSeconds(1f);

        currentWaste.SetParent(carryPosition);
        currentWaste.localPosition = Vector3.zero;

        agent.SetDestination(incorrectTrashBin.position);
        while (Vector3.Distance(transform.position, incorrectTrashBin.position) > 1.2f)
            yield return null;

        npcAnimator.SetTrigger("Sort");
        yield return new WaitForSeconds(1.5f);

        if (incorrectBinComponent != null && incorrectBinComponent.TryAddRubbish())
        {
            feedbackText.text = "I did it!";
            nearbyWaste.Remove(currentWaste);
            Destroy(currentWaste.gameObject);
        }

        currentWaste = null;
        hasDisposedIncorrectly = true;
        isSorting = false;
    }

    IEnumerator SortWasteCorrectly()
    {
        isSorting = true;
        currentWaste = FindNearestWaste();
        if (currentWaste == null)
        {
            feedbackText.text = "Nothing left to sort!";
            isSorting = false;
            yield break;
        }

        agent.SetDestination(currentWaste.position);
        while (Vector3.Distance(transform.position, currentWaste.position) > 1.2f)
            yield return null;

        npcAnimator.SetTrigger("Pick");
        yield return new WaitForSeconds(1f);

        currentWaste.SetParent(carryPosition);
        currentWaste.localPosition = Vector3.zero;

        agent.SetDestination(correctTrashBin.position);
        while (Vector3.Distance(transform.position, correctTrashBin.position) > 1.2f)
            yield return null;

        npcAnimator.SetTrigger("Sort");
        yield return new WaitForSeconds(1.5f);

        if (correctBinComponent != null && correctBinComponent.TryAddRubbish())
        {
            if (rubbishTracker != null)
                rubbishTracker.AddCorrectDisposal();

            feedbackText.text = "All sorted now!";
            nearbyWaste.Remove(currentWaste);
            Destroy(currentWaste.gameObject);
        }

        currentWaste = null;
        isSorting = false;
    }

    Transform FindNearestWaste()
    {
        float shortest = Mathf.Infinity;
        Transform nearest = null;

        foreach (Transform waste in nearbyWaste)
        {
            if (waste != null)
            {
                float dist = Vector3.Distance(transform.position, waste.position);
                if (dist < shortest)
                {
                    shortest = dist;
                    nearest = waste;
                }
            }
        }

        return nearest;
    }

    public bool HasDisposedIncorrectly() => hasDisposedIncorrectly;
    public bool IsEducated() => isEducated;
}
