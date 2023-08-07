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

    /// <summary>
    /// Deletes the patient when they enter the objects trigger.
    /// This function may require conditionals, so patients are not deleted by accident.
    /// </summary>
    /// <param name="other"></param>
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
