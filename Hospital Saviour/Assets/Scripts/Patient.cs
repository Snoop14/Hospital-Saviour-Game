using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Patient : MonoBehaviour
{
    public bool isInteractable = false;
    //bool to hold if carrying folder
    public bool isHoldingFolder = false;
    public GameObject folder = null;
    //for bed or machines
    GameObject assignedPlacement = null;

    public Vector3 queuePosition;

    //array to hold states of this patient type
    private string[] actions = { "waiting", "walking" };

    //integer to hold current position in the state array
    private int actionsVal;

    //string to hold current state
    private string currState;

    Rigidbody rbody;
    float speed;
    Vector3 targetPosition;

    public Image iconPrefab; // Assign this in the Inspector
    Image icon;
    public Sickness sickness;
    Image sicknessIconBackground;
    Image sicknessIcon;
    Image healingIcon;
    List<Sprite> healingOrderIcons;
    //int to hold position in icons
    private int iconPos = 0;

    void Start()
    {
        speed = 7.0f;
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
        healingIcon.sprite = healingOrderIcons[iconPos];
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
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 lookAtTarget = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.LookAt(lookAtTarget);
        rbody.velocity = direction * speed;
    }
    //set initial values
    private void setState()
    {
        actionsVal = 0;
        currState = actions[actionsVal];
        //Debug.Log(currState);
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

        //Debug.Log(currState);
    }

    public void releaseFolder()
    {
        isHoldingFolder = false;
        //folder = null; //is this okay to be null, this way we still know exactly which folder is actually the patients
    }

    public void folderPlaced(GameObject place)
    {
        assignedPlacement = place;
        targetPosition = assignedPlacement.transform.position - new Vector3(0, 0, 1.0f);
        iterateState(); //Due to changes in the action array we may need to change this
    }

    public void moveInQueue(Vector3 pos)
    {
        queuePosition = pos;
        targetPosition = queuePosition;
        Debug.Log(targetPosition);
        Debug.Log("moving func called");
    }

    private void OnCollisionEnter(Collision other)
    {
        //Due to changes made elsewhere (actions array), this will likely also need to be changed
        /*
        if(other.gameObject.CompareTag("Bed"))
        {
            iterateState(); // set state to bed 
            print("Patient: " + other.gameObject.name + " entered collider");
        }
        */

        if (other.gameObject.TryGetComponent(out Bed b))
        {
            if (b.currentFolder == folder)
            {
                b.NPCInteract(gameObject);
                changePosToBed();
            }
        }
    }

    public void changePosToBed()
    {
        //iterateState(); // set state to waiting //Due to changes in the action array we may need to change this

        currState = actions[0];

        //Debug.Log("Patient: Switched to bed");
        transform.parent = assignedPlacement.transform; //changes the parent of folder to the transfered object
        transform.localPosition = new Vector3(0f, 1f, 0f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
        transform.localRotation = Quaternion.Euler(-90f, 90f, 180f); //resets rotation
    }

    public GameObject getAssignment<GameObject>()
    {
        return assignedPlacement.GetComponent<GameObject>();
    }

    //managed interactions with player whilst on bed
    public void interactionOnBed()
    {
        Debug.Log("I'm Occupied");

        //change icon
        //Debug.Log(healingOrderIcons.Count);
        //healingIcon.sprite = healingOrderIcons[1];

        iterateIcons();

        //change state

    }

    private void iterateIcons() 
    {
        //Debug.Log(icon.sprite);
        Debug.Log(iconPrefab);
        if (icon.sprite == sickness.sicknessIcon)
        {
            Debug.Log("If");
            icon = healingIcon;
        }
        else
        {
            //need to add a checker later on to prevent going out of range
            Debug.Log("else");
            //iconPos += 1;
            healingIcon.sprite = healingOrderIcons[iconPos];
            icon = healingIcon;
        }


    }
}
