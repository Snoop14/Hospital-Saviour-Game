using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Patient : MonoBehaviour
{
    private NavMeshAgent agent;

    public bool isInteractable = false;
    //bool to hold if carrying folder
    public bool isHoldingFolder = false;
    public GameObject folder = null;
    //for bed or machines
    GameObject assignedPlacement = null;

    public Vector3 queuePosition;

    //array to hold states of this patient type
    private string[] actions = { "waiting", "walking"};

    //integer to hold current position in the state array
    private int actionsVal;

    //string to hold current state
    private string currState;

    Rigidbody rbody;

    Vector3 targetPosition;

    public Image iconPrefab; // Assign this in the Inspector
    Image icon;
    public Sickness sickness;
    Image sicknessIconBackground;
    Image sicknessIcon;
    Image healingIcon;
    List<Sprite> healingOrderIcons;

    private int currHeal = 0;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        targetPosition = transform.position;
        rbody = GetComponent<Rigidbody>();
        icon = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform);
        
        sicknessIconBackground = icon.transform.GetChild(0).GetComponent<Image>();
        sicknessIcon = icon.transform.GetChild(1).GetComponent<Image>();
        sicknessIcon.SetNativeSize();
        //sicknessIcon.transform.localScale = new Vector3(0.3f,0.3f,1);
        healingIcon = icon.transform.GetChild(2).GetComponent<Image>();
        healingOrderIcons = sickness.healingOrderIcons;

        //change based on patient type
        //if (sickness.type == "Germ") //It will change regardless so I am unsure as to whether this is needed?
        //{
        sicknessIconBackground.sprite = sickness.sicknessIconBackGround;
        sicknessIcon.sprite = sickness.sicknessIcon;
        healingIcon.sprite = healingOrderIcons[currHeal];
        healingIcon.SetNativeSize();
        //}

        setState();
    }
    void Update()
    {
        Vector3 offset = new Vector3(0, 2.5f, 0);
        Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(transform.position + offset);
        icon.transform.position = positionOnScreen;
    }
    void FixedUpdate()
    {
        if (currState == actions[1])  // walking
        {
            move();
        }
        else
        {
            rbody.velocity = Vector3.zero;
        }
    }

    public void move()
    {
        agent.SetDestination(targetPosition);
    }
    //set initial values
    private void setState()
    {
        actionsVal = 0;
        currState = actions[actionsVal];
        Debug.Log(currState);
    }

    public string getState()
    {
        return currState;
    }

    //move to next state
    private void iterateState() //Due to changes in the action array we will need to change this
    {
        //add 1 to the current position
        actionsVal += 1;
        //change the state
        currState = actions[actionsVal];

        Debug.Log(currState);
    }

    public void releaseFolder()
    {
        isHoldingFolder = false;
        //folder = null; //is this okay to be null, this way we still know exactly which folder is actually the patients
    }

    public void folderPlaced(GameObject place)
    {
        assignedPlacement = place;
        targetPosition = assignedPlacement.transform.position - new Vector3(0,0,1.0f);
        agent.SetDestination(targetPosition);
    }

    public void moveInQueue(Vector3 pos)
    {
        queuePosition = pos;
        targetPosition = queuePosition;
        agent.SetDestination(targetPosition);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.TryGetComponent(out Bed b))
        {
            if(b.currentFolder == folder)
            {
                b.NPCInteract(gameObject);
                changePosToBed();
                icon.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }

    public void changePosToBed()
    {
        //iterateState(); // set state to waiting //Due to changes in the action array we may need to change this

        currState = actions[0];

        Debug.Log("Patient: Switched to bed");
        agent.enabled = false;//Disable AI movement
        transform.parent = assignedPlacement.transform; //changes the parent of folder to the transfered object
        transform.localPosition = new Vector3(0f, 1f, 0f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        transform.localRotation = Quaternion.Euler(-90f, 90f, 180f); //resets rotation
    }

    public GameObject getAssignment<GameObject>()
    {
        return assignedPlacement.GetComponent<GameObject>();
    }

    //Called when an object is given to the patient while they are on the bed
    public void healOnBed(string item)
    {
        if(item == healingIcon.sprite.name) 
        {
            currHeal++;
        }

        if (currHeal == healingOrderIcons.Count)
        {
            StartCoroutine(leaveBed(assignedPlacement));
        }
        else
        {
            healingIcon.sprite = healingOrderIcons[currHeal];
        }
    }


    IEnumerator leaveBed(GameObject bed)
    {
        yield return new WaitForSeconds(1.5f);
        Bed b = bed.GetComponent<Bed>();
        b.FolderPickUp();
        b.NPCLeaves();
        folder.GetComponent<Folder>().destroySelf();
        leaveHospital();
    }
    //called when patient is to leave the hospital
    private void leaveHospital()
    {
        agent.enabled = true;
        icon.gameObject.SetActive(false);
        Vector3 leaveLoc = new Vector3(-13.5f, 0.5f, 13.5f);
        agent.SetDestination(leaveLoc);
    }
}
