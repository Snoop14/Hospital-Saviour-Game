﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;
    Vector3 movementVec;
    
    private Rigidbody rbody;

    [SerializeField]
    public GameObject gameManager;
    GameManager manager;

    public GameObject healingIconObject;

    public bool isCarrying = false;
    string itemType = "";
    GameObject item = null;

    List<GameObject> collidingObjects;

    public tutorial tutorial;

    //Events"
    public delegate void InteractWithPatient();
    public event InteractWithPatient OnInteractWithPatient; 
    public delegate void InteractWithSoupMachine();
    public event InteractWithSoupMachine OnInteractWithSoupMachine;
    public delegate void InteractWithPillMachine();
    public event InteractWithPillMachine OnInteractWithPillMachine;

    void Start()
    {
        transform.localPosition = new Vector3(0, 1, 0);
        rbody = GetComponent<Rigidbody>();
        collidingObjects = new List<GameObject>();
        manager = gameManager.GetComponent<GameManager>();
        healingIconObject = GameObject.Find("PlayerIcon");
        healingIconObject.GetComponent<Image>().SetNativeSize();
        healingIconObject.transform.localScale = new Vector3(0.3f, 0.3f, 1);
        healingIconObject.SetActive(false);
    }

    void FixedUpdate()
    {
        //Gets player to look in direction of movement
        transform.LookAt(transform.position + movementVec, new Vector3(0, 1, 0));
        rbody.velocity = movementVec * speed;

        if (healingIconObject.activeSelf)
        {
            Vector3 offset = new Vector3(0, 2.5f, 1.5f);
            Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position + offset);
            healingIconObject.transform.position = positionOnScreen;
        }
        
    }

    /// <summary>
    /// Called when the interact button is pressed.
    /// </summary>
    public void OnToggleTutorial()
    {
        tutorial.changeActive();
    }

    /// <summary>
    /// Called when the movement keys are pressed.
    /// </summary>
    /// <param name="input"></param>
    public void OnMove(InputValue input)
    {
        movementVec = Vector3.zero;
        Vector2 xyInput = input.Get<Vector2>();
        if (tutorial.gameObject.activeSelf)
        {
            if (xyInput.x > 0)
                tutorial.nextPage();
            else if(xyInput.x < 0)
                tutorial.prevPage();
            return;
        }
        movementVec = new Vector3(xyInput.x, 0, xyInput.y);
    }

    /// <summary>
    /// Called when the interact button is pressed.
    /// Checks multiple things, such as if the player is carrying, if so what is being carried
    /// and what the object is that the player is interacting with
    /// to then decide what functions to call
    /// </summary>
    public void OnInteract()
    {
        if (tutorial.gameObject.activeSelf)
            return;
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
                                
                                //change to patient 
                                f.transferTo(go);
                                f.changePosToPatient();
                                p.retakeFolder();
                                
                                isCarrying = false;
                                item = null;
                                itemType = "";


                                OnInteractWithPatient?.Invoke();



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
                            Patient patient = b.currentPatient.GetComponent<Patient>();
                            Soup s = item.GetComponent<Soup>();
                            s.transferTo(patient.gameObject);
                            s.changePosToBed();
                            StartCoroutine(s.destroySelf());

                            //Stuff needs to be done here to actually continue with soup hand off
                            patient.healOnBed(itemType);

                            isCarrying = false;
                            item = null;
                            itemType = "";

                            break;
                        }

                        Bin t = go.GetComponent<Bin>();
                        if (t)
                        {
                            Soup s = item.GetComponent<Soup>();
                            s.transferTo(go);
                            StartCoroutine(s.destroySelf());
                            
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
                            Patient patient = b.currentPatient.GetComponent<Patient>();
                            Pill p = item.GetComponent<Pill>();
                            p.transferTo(patient.gameObject);
                            p.changePosToBed();
                            StartCoroutine(p.destroySelf());

                            //Stuff needs to be done here to actually continue with soup hand off
                            patient.healOnBed(itemType);

                            isCarrying = false;
                            item = null;
                            itemType = "";
                            break;
                        }


                        Bin t = go.GetComponent<Bin>();
                        if (t)
                        {
                            Pill p = item.GetComponent<Pill>();
                            p.transferTo(go);
                            StartCoroutine(p.destroySelf());

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

                        Bin t = go.GetComponent<Bin>();
                        if (t)
                        {
                            Bandage band = item.GetComponent<Bandage>();
                            band.transferTo(go);
                            StartCoroutine(band.destroySelf());

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
                    //Adjusted the getComponent to TryGetComponent
                    if (go.TryGetComponent(out Patient p))
                    {
                        if (p && p.isInteractable)
                        {
                            if (p.isHoldingFolder)
                            {
                                item = p.folder;
                                if (item != null)
                                {
                                    isCarrying = true;
                                    itemType = "Folder";
                                    p.releaseFolder();
                                    Folder i = item.GetComponent<Folder>();
                                    i.transferTo(gameObject);
                                    i.changePosToPlayer();


                                    OnInteractWithPatient?.Invoke();
                                }

                                break;
                            }
                        }
                    }

                    if(go.TryGetComponent(out SoupMachine s))
                    {
                        item = s.currentSoup;


                        if (item != null)
                        {
                            isCarrying = true;
                            itemType = "Soup";
                            s.soupPickUp();
                            Soup i = item.GetComponent<Soup>();
                            i.transferTo(gameObject);
                            i.changePosToPlayer();


                            OnInteractWithSoupMachine?.Invoke();
                        }
                        break;
                    }

                    if (go.TryGetComponent(out PillMachine pillM))
                    {
                        item = pillM.currentPill;

                        if (item != null)
                        {
                            isCarrying = true;
                            itemType = "Pill";
                            pillM.pillPickUp();
                            Pill i = item.GetComponent<Pill>();
                            i.transferTo(gameObject);
                            i.changePosToPlayer();

                            OnInteractWithPillMachine?.Invoke();
                        }
                        break;

                    }

                    if (go.TryGetComponent(out BandageMachine bandM))
                    {
                        item = bandM.currentBandage;

                        if (item != null)
                        {
                            isCarrying = true;
                            itemType = "Bandage";
                            bandM.bandagePickUp();
                            Bandage i = item.GetComponent<Bandage>();
                            i.transferTo(gameObject);
                            i.changePosToPlayer();
                        }
                        break;
                    }

                    if (go.TryGetComponent(out Bed b))
                    {
                        b.interactWithPatient(isCarrying, gameObject);

                        OnInteractWithPatient?.Invoke();

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

    /// <summary>
    /// Display an icon above the player upon interacting with patient to find out what patient needs
    /// </summary>
    /// <param name="s"></param>
    public void IconChange(Sprite s)
    {
        healingIconObject.GetComponent<Image>().sprite = s;
        healingIconObject.SetActive(true);
        StartCoroutine(HideIcon());
    }

    /// <summary>
    /// Hide icon above player
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideIcon()
    {
        yield return new WaitForSeconds(2f);
        healingIconObject.SetActive(false);
    }
}