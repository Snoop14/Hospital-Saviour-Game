﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player1 : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 movementVec;
    
    private Rigidbody rbody;

    [SerializeField]
    GameObject gameManager;
    GameManager manager;

    public bool isCarrying = false;
    string itemType = "";
    GameObject item = null;

    List<GameObject> collidingObjects;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        collidingObjects = new List<GameObject>();
        manager = gameManager.GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        //Gets player to look in direction of movement
        transform.LookAt(transform.position + movementVec, new Vector3(0, 1, 0));
        rbody.velocity = movementVec * speed; 
    }

    public void OnMove(InputValue input)
    {
        Vector2 xyInput = input.Get<Vector2>();
        movementVec = new Vector3(xyInput.x, 0, xyInput.y);
    }

    public void OnInteract()
    {
        if (isCarrying)
        {
            if(itemType == "Folder")
            {
                foreach (GameObject go in collidingObjects)
                {
                    if (manager.objectList.Contains(go))
                    {
                        Bed b = go.GetComponent<Bed>();
                        if (b && b.isActive && b.isInteractable && !b.hasFolder && !b.isOccupied)
                        {
                            Folder f = item.GetComponent<Folder>();
                            f.transferTo(go);
                            f.changePosToBed();
                            b.folderDropOff(item);
                            f.patientOwner.GetComponent<Patient>().folderPlaced(go);
                            manager.removeFromQueue(f.patientOwner);
                            isCarrying = false;
                            item = null;
                            itemType = "";

                            break;
                        }

                        Patient p = go.GetComponent<Patient>();
                        //may also need a check to confirm folder belongs to this patient
                        if (p && p.isInteractable && p.isInQueue && !p.isHoldingFolder)
                        {
                            Folder f = item.GetComponent<Folder>();

                            //check if the patient is the owner of the notes
                            if (f.patientOwner.GetComponent<Patient>() == p)
                            {

                                Debug.Log("true");
                                
                                //change to patient 
                                f.transferTo(go);
                                f.changePosToPatient();

                                //b.folderDropOff(item);
                                //f.patientOwner.GetComponent<Patient>().folderPlaced(go);
                                //manager.removeFromQueue(f.patientOwner);

                                isCarrying = false;
                                item = null;
                                itemType = "";

                                break;
                            }
                            
                        }

                    }
                }
            }
            
            else if (itemType == "Soup")
            {
                foreach (GameObject go in collidingObjects)
                {
                    if (manager.objectList.Contains(go))
                    {
                        Bed b = go.GetComponent<Bed>();
                        if (b && b.isActive && b.isInteractable && b.hasFolder && b.isOccupied)
                        {
                            Soup s = item.GetComponent<Soup>();
                            s.transferTo(go);
                            s.changePosToBed();
                            StartCoroutine(s.destroySelf());

                            //Stuff needs to be done here to actually continue with soup hand off
                            b.currentPatient.GetComponent<Patient>().healOnBed(itemType);

                            isCarrying = false;
                            item = null;
                            itemType = "";
                            break;
                        }

                        Bin t = go.GetComponent<Bin>();
                        if (t)
                        {
                            Soup s = item.GetComponent<Soup>();
                            //s.transferTo(go);
                            //s.changePosToBed();
                            StartCoroutine(s.destroySelf());
                            Debug.Log("destroying Soup");
                            //Stuff needs to be done here to actually continue with soup hand off

                            isCarrying = false;
                            item = null;
                            itemType = "";
                            break;
                        }
                    }
                }
            }
            else if (itemType == "Pill")
            {
                foreach (GameObject go in collidingObjects)
                {
                    if (manager.objectList.Contains(go))
                    {
                        Bed b = go.GetComponent<Bed>();
                        if (b && b.isActive && b.isInteractable && b.hasFolder && b.isOccupied)
                        {
                            Pill p = item.GetComponent<Pill>();
                            p.transferTo(go);
                            p.changePosToBed();
                            StartCoroutine(p.destroySelf());

                            //Stuff needs to be done here to actually continue with soup hand off
                            b.currentPatient.GetComponent<Patient>().healOnBed(itemType);

                            isCarrying = false;
                            item = null;
                            itemType = "";
                            break;
                        }
                    }
                }
            }
            else if (itemType == "Bandage")
            {
                foreach (GameObject go in collidingObjects)
                {
                    if (manager.objectList.Contains(go))
                    {
                        Bed b = go.GetComponent<Bed>();
                        if (b && b.isActive && b.isInteractable && b.hasFolder && b.isOccupied)
                        {
                            Bandage band = item.GetComponent<Bandage>();
                            band.transferTo(go);
                            band.changePosToBed();
                            StartCoroutine(band.destroySelf());

                            //Stuff needs to be done here to actually continue with soup hand off
                            b.currentPatient.GetComponent<Patient>().healOnBed(itemType);

                            isCarrying = false;
                            item = null;
                            itemType = "";
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (GameObject go in collidingObjects)
            {
                if (manager.objectList.Contains(go))
                {
                    //Patient p = go.GetComponent<Patient>();
                    //print(p);
                    //Adjusted the getComponent to TryGetComponent
                    if (go.TryGetComponent(out Patient p))
                    {
                        if (p && p.isInteractable)
                        {
                            if (p.isHoldingFolder)
                            {
                                isCarrying = true;
                                item = p.folder;
                                itemType = "Folder";
                                p.releaseFolder();
                                Folder i = item.GetComponent<Folder>();
                                i.transferTo(gameObject);
                                i.changePosToPlayer();

                                break;
                            }
                        }
                    }

                    if(go.TryGetComponent(out SoupMachine s))
                    {
                        isCarrying = true;
                        item = s.currentSoup;
                        itemType = "Soup";
                        s.soupPickUp();
                        Soup i = item.GetComponent<Soup>();
                        i.transferTo(gameObject);
                        i.changePosToPlayer();

                        break;
                    }

                    if (go.TryGetComponent(out PillMachine pillM))
                    {
                        isCarrying = true;
                        item = pillM.currentPill;
                        itemType = "Pill";
                        pillM.pillPickUp();
                        Pill i = item.GetComponent<Pill>();
                        i.transferTo(gameObject);
                        i.changePosToPlayer();

                        break;
                    }

                    if(go.TryGetComponent(out BandageMachine bandM))
                    {
                        isCarrying = true;
                        item = bandM.currentBandage;
                        itemType = "Bandage";
                        bandM.bandagePickUp();
                        Bandage i = item.GetComponent<Bandage>();
                        i.transferTo(gameObject);
                        i.changePosToPlayer();
                    }

                    if (go.TryGetComponent(out Bed b))
                    {
                        b.interactWithPatient(isCarrying);

                        break;
                    }
                }
            }
        }
    }

    

    //Called when the player enters in range of another object
    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            collidingObjects.Add(other.gameObject);
            //print(other.gameObject.name + " entered collider");
        }
    }

    //Called when the player exits range of another object
    private void OnTriggerExit(Collider other)
    {
        if (collidingObjects.Contains(other.gameObject))
        {
            collidingObjects.Remove(other.gameObject);
            //print(other.gameObject.name + " left collider");
        }
    }

}


