using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupMachine : MonoBehaviour
{

    [SerializeField] GameObject soupPrefab;

    public GameObject currentSoup { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(generateSoup());
    }

    /// <summary>
    /// Called when player picks up the soup
    /// </summary>
    public void soupPickUp()
    {
        currentSoup = null;
        StartCoroutine(generateSoup());
    }

    /// <summary>
    /// Regenerates soup after a certain amount of time
    /// </summary>
    IEnumerator generateSoup()
    {
        yield return new WaitForSeconds(1.0f);
        Vector3 spawnLoc = new Vector3(transform.localPosition.x, 
                                       transform.localPosition.y + 1.69f, 
                                       transform.localPosition.z);
        Quaternion spawnRot = new Quaternion();
        currentSoup = Instantiate(soupPrefab, spawnLoc, spawnRot, transform);
    }
}
