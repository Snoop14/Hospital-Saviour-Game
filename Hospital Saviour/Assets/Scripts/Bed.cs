using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    public bool isActive = false;
    public bool isOccupied = false;
    public bool isInteractable = false;
    public bool hasFolder = false;
    public GameObject currentPatient { get; private set; } = null;
    public GameObject currentFolder { get; private set; } = null;

    /// <summary>
    /// Folder is dropped off here
    /// </summary>
    /// <param name="folder"></param>
    public void folderDropOff(GameObject folder)
    {
        //sets folder variable to be the folder
        currentFolder = folder;

        //marks the bed as having a folder, so no other folders can be added
        hasFolder = true;
    }

    /// <summary>
    /// Folder is picked up/removed from here
    /// </summary>
    public void FolderPickUp()
    {
        //resets current folder variable
        currentFolder = null;
        //resets hasFolder variable - so folder can be added again
        hasFolder = false;
    }

    /// <summary>
    /// Patient gets on bed
    /// </summary>
    /// <param name="patient"></param>
    public void NPCInteract(GameObject patient)
    {
        //sets purrent patient
        currentPatient = patient;
        
        //marks the bed as occupied
        isOccupied = true;
    }

    /// <summary>
    /// Patient leaves the bed
    /// </summary>
    public void NPCLeaves()
    {
        //resets current patient variable
        currentPatient = null;
        //resets occupied variable - so bed an be occupied again
        isOccupied = false;
    }

    public void interactWithPatient(ItemType t, GameObject player)
    {
        if (isOccupied)
        {
            //gets the beds current patient rather than searching through the children
            currentPatient.GetComponent<Patient>().interactionOnBed(t, player);
        }
    }
}
