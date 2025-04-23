using UnityEngine;

public class PlayerEducator : MonoBehaviour
{
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryEducateNPC();
        }
    }

    void TryEducateNPC()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (Collider hit in hits)
        {
            NPCWasteBehavior npc = hit.GetComponent<NPCWasteBehavior>();
            if (npc != null)
            {
                npc.InteractWithPlayer();
                break;
            }
        }
    }
}
