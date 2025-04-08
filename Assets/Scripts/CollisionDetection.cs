using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    [SerializeField] GameObject thePlayer;
    private void OnTriggerEnter(Collider other)
    {
        thePlayer.GetComponent<PlayerMovement>().enabled = false;
        thePlayer.GetComponent<PlayerController>().enabled = false;
    }
}
