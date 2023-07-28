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
        generateSoup();
    }

    public void soupPickUp()
    {
        currentSoup = null;
        generateSoup();
    }

    //Generates a new soup at the specific location
    void generateSoup()
    {
        Vector3 spawnLoc = new Vector3(transform.localPosition.x, 
                                       transform.localPosition.y + 1.1255f, 
                                       transform.localPosition.z - 0.1f);
        Quaternion spawnRot = new Quaternion();
        currentSoup = Instantiate(soupPrefab, spawnLoc, spawnRot, transform);
    }
}
