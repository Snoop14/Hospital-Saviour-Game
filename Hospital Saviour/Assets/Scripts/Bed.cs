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
        currentFolder = folder;
        hasFolder = true;
    }

    /// <summary>
    /// Folder is picked up/removed from here
    /// </summary>
    public void FolderPickUp()
    {
        currentFolder = null;
        hasFolder = false;
    }

    /// <summary>
    /// Patient gets on bed
    /// </summary>
    /// <param name="patient"></param>
    public void NPCInteract(GameObject patient)
    {
        currentPatient = patient;
        isOccupied = true;
        //isActive = true; // Please confirm
        //Patient should only be able to get on a bed that is already active
    }

    /// <summary>
    /// Patient leaves the bed
    /// </summary>
    public void NPCLeaves()
    {
        currentPatient = null;
        isOccupied = false;
    }

    public void interactWithPatient(bool isCarrying, GameObject player)
    {
        if (isOccupied)
        {
            //gets the beds current patient rather than searching through the children
            currentPatient.GetComponent<Patient>().interactionOnBed(isCarrying, player);
            //GetComponentInChildren<Patient>().interactionOnBed(isCarrying);
        }
    }
}
