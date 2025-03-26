using System.Collections;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public GameObject[] segmentMaps; // Array to store all segment maps

    void Start()
    {
        StartCoroutine(SegmentGen());
    }

    IEnumerator SegmentGen()
    {
        for (int i = 1; i < segmentMaps.Length; i++) // Start from index 1 since index 0 is already active
        {
            yield return new WaitForSeconds(10);
            segmentMaps[i].SetActive(true);
        }
    }
}
