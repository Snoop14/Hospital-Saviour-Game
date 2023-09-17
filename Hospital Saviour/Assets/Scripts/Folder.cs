using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folder : Item
{
    public GameObject patientOwner;

    /// <summary>
    /// Changes position of folder to the players hands
    /// </summary>
    public new void changePosToPlayer()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }
    /// <summary>
    /// Changes position of self to end of bed 
    /// </summary>
    public new void changePosToBed()
    {
        transform.localPosition = new Vector3(-1.650f, 0.15f, 0f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        transform.localRotation = Quaternion.Euler(-90f, 90f, 0f); //resets rotation
    }

    /// <summary>
    /// Changes position of folder to the players hands
    /// </summary>
    public void changePosToPatient()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }
}
