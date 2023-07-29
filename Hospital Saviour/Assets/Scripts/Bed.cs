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

    private void FolderPickUp()
    {
        currentFolder = null;
    }

    public void NPCInteract(GameObject patient)
    {
        Debug.Log("Bed: Assigned patient");
        currentPatient = patient;
        isOccupied = true;
        isActive = true; // Please confirm
    }
}
