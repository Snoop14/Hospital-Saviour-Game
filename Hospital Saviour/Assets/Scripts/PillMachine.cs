using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillMachine : MonoBehaviour
{
    [SerializeField] GameObject pillPrefab;

    public bool isInteractable = true;


    public GameObject currentPill { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(generatePill());
    }

    /// <summary>
    /// Called when player picks up the pill
    /// </summary>
    public void pillPickUp()
    {
        currentPill = null;
        StartCoroutine(generatePill());
    }

    /// <summary>
    /// Regenerates pills after a certain amount of time
    /// </summary>
    IEnumerator generatePill()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 spawnLoc = new Vector3(transform.localPosition.x,
                                       transform.localPosition.y + 1.2f,
                                       transform.localPosition.z - 0.9f);
        Quaternion spawnRot = new Quaternion();
        currentPill = Instantiate(pillPrefab, spawnLoc, spawnRot, transform);
    }
}
