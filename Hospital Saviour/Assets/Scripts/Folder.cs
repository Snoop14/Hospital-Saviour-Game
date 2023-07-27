using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folder : MonoBehaviour
{
    public GameObject patientOwner;
    public void transferTo(GameObject obj)
    {
        transform.parent = obj.transform; //changes the parent of folder to the transfered object
    }

    public void changePosToPlayer()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }
    public void changePosToBed()
    {
        transform.localPosition = new Vector3(0.8f, 0.4f, -0.763f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        transform.localRotation = Quaternion.Euler(0f, 90f, -90f); //resets rotation
    }
    //change pos to machine... xray 
}
