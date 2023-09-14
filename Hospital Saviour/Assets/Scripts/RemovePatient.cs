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
    /// Destroys the patient when they enter the objects trigger.
    /// Tells manager to end game if all patients are destroyed
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Patient")
        {
            manager.RemovePatientFromList(other.gameObject);
            other.GetComponent<Patient>().DestroySelf();
            removeCounter++;
        }

        if(removeCounter == patientCount)
        {
            manager.EndGame();
        }
    }
}
