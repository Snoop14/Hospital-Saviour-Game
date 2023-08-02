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


    public void folderDropOff(GameObject folder)
    {
        currentFolder = folder;
        hasFolder = true;
    }

    public void FolderPickUp()
    {
        currentFolder = null;
        hasFolder = false;
    }

    public void NPCInteract(GameObject patient)
    {
        currentPatient = patient;
        isOccupied = true;
        //isActive = true; // Please confirm
        //Patient should only be able to get on a bed that is already active
    }

    public void NPCLeaves()
    {
        currentPatient = null;
        isOccupied = false;
    }

    public void interactWithPatient(bool isCarrying)
    {
        if (isOccupied)
        {
            GetComponentInChildren<Patient>().interactionOnBed(isCarrying);
        }
    }
}
