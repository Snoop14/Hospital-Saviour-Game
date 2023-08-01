using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandageMachine : MonoBehaviour
{
    [SerializeField] GameObject bandagePrefab;

    public GameObject currentBandage { get; private set; } = null;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(generateBandage());
    }

    public void bandagePickUp()
    {
        currentBandage = null;
        StartCoroutine(generateBandage());
    }

    IEnumerator generateBandage()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 spawnLoc = new Vector3(transform.localPosition.x,
                                       transform.localPosition.y + 0.95f,
                                       transform.localPosition.z);
        Quaternion spawnRot = new Quaternion();
        spawnRot.eulerAngles = new Vector3(90, 0, 0);
        currentBandage = Instantiate(bandagePrefab, spawnLoc, spawnRot, transform);
    }
}
