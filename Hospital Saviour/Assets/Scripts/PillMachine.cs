﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillMachine : MonoBehaviour
{
    [SerializeField] GameObject pillPrefab;

    public GameObject currentPill { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(generatePill());
    }

    public void pillPickUp()
    {
        currentPill = null;
        StartCoroutine(generatePill());
    }
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
