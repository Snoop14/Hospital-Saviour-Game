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
        //find the game manager and set it into the manager variable
        manager = GameObject.Find("Manager").GetComponent<GameManager>();

        //set the patient count variable to be the same as the patient count in the manager
        patientCount = manager.patientCount;

        //set the alue of remove counter to be 0
        removeCounter = 0;
    }

    /// <summary>
    /// Destroys the patient when they enter the objects trigger.
    /// Tells manager to end game if all patients are destroyed
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //if it has the patient tag...
        if(other.tag == "Patient")
        {
            //remove the patient from the manager list of objects
            manager.RemovePatientFromList(other.gameObject);

            //destroys the patient
            other.GetComponent<Patient>().DestroySelf();

            //increase value of removecounter variable
            removeCounter++;
        }

        //if the remve counter variable is the same as the count of the patients ...
        if(removeCounter == patientCount)
        {
            //runf the endgame function in the manager
            manager.EndGame();
        }
    }
}
