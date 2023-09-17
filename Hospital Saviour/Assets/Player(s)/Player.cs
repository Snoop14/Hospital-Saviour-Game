using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;
    Vector3 movementVec;
    
    private Rigidbody rbody;

    public GameObject healingIconObject;

    ItemType itemType = ItemType.Empty;
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
        foreach (GameObject go in collidingObjects)
        {
            if (itemType != ItemType.Empty)
            {
                if (itemType == ItemType.Folder)
                {
                    
                    Bed b = go.GetComponent<Bed>();
                    if (b && b.isActive && b.isInteractable && !b.hasFolder && !b.isOccupied)
                    {
                        Folder f = item.GetComponent<Folder>();
                        f.transferTo(go);
                        f.changePosToBed();
                        b.folderDropOff(item);
                        f.patientOwner.GetComponent<Patient>().folderPlaced(go);
                        item = null;
                        itemType = ItemType.Empty;

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
                            item = null;
                            itemType = ItemType.Empty;


                            OnInteractWithPatient?.Invoke();

                            break;
                        }

                    }
                }

                else
                {
                    Bed b = go.GetComponent<Bed>();
                    if (b && b.isActive && b.isInteractable && b.hasFolder && b.isOccupied)
                    {
                        OnInteractWithItem(b);

                        break;
                    }

                    Bin bin = go.GetComponent<Bin>();
                    if (bin)
                    {
                        Item s = item.GetComponent<Item>();
                        s.transferTo(go);
                        StartCoroutine(s.destroySelf());
                        item = null;
                        itemType = ItemType.Empty;

                        break;
                    }
                }
            }
            else
            {
                //Adjusted the getComponent to TryGetComponent
                if (go.TryGetComponent(out Patient p))
                {
                    if (p && p.isInteractable)
                    {
                        if (p.isHoldingFolder)
                        {
                            item = p.folder;
                            itemType = ItemType.Folder;
                            p.releaseFolder();
                            Folder i = item.GetComponent<Folder>();
                            i.transferTo(gameObject);
                            i.changePosToPlayer();
                            OnInteractWithPatient?.Invoke();

                            break;
                        }
                    }
                }

                if (go.TryGetComponent(out SoupMachine s))
                {
                    item = s.currentSoup;


                    if (item != null)
                    {
                        itemType = ItemType.Soup;
                        s.soupPickUp();
                        Item i = item.GetComponent<Item>();
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
                        itemType = ItemType.Pill;
                        pillM.pillPickUp();
                        Item i = item.GetComponent<Item>();
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
                        itemType = ItemType.Bandage;
                        bandM.bandagePickUp();
                        Item i = item.GetComponent<Item>();
                        i.transferTo(gameObject);
                        i.changePosToPlayer();
                    }
                    break;
                }

                if (go.TryGetComponent(out Bed b))
                {
                    b.interactWithPatient(itemType, gameObject);
                    OnInteractWithPatient?.Invoke();

                    break;
                }
            }
        }
    }

    void OnInteractWithItem(Bed b)
    {
        Patient patient = b.currentPatient.GetComponent<Patient>();
        if (patient.isInteractable)
        {
            Item s = item.GetComponent<Item>();
            s.transferTo(patient.gameObject);
            s.changePosToBed();
            item = null;
            itemType = ItemType.Empty;
            StartCoroutine(s.destroySelf());

            patient.healOnBed(itemType);
            
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