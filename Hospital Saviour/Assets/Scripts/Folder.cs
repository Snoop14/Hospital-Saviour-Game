using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Folder : MonoBehaviour
{
    public GameObject patientOwner;

    /// <summary>
    /// Transfers the object to be a child of another
    /// </summary>
    /// <param name="obj"></param>
    public void transferTo(GameObject obj)
    {
        transform.parent = obj.transform; //changes the parent of folder to the transfered object
    }

    /// <summary>
    /// Changes position of folder to the players hands
    /// </summary>
    public void changePosToPlayer()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }
    /// <summary>
    /// Changes position of self to end of bed 
    /// </summary>
    public void changePosToBed()
    {
        transform.localPosition = new Vector3(-1.5f, 0.5f, 0f);
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


    /// <summary>
    /// Destroy self 
    /// </summary>
    public void destroySelf()
    {
        Destroy(gameObject);
    }
    //change pos to machine... xray 
}
