using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovePatient : MonoBehaviour
{
    GameManager manager;
    int patientCount;
    int removeCounter;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<GameManager>();
        patientCount = manager.patientCount;
        removeCounter = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Patient")
        {
            other.GetComponent<Patient>().DestroySelf();
            removeCounter++;
        }

        if(removeCounter == patientCount)
        {
            manager.EndGame();
        }
    }
}
