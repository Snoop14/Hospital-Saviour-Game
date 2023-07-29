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
    private string[] actions = { "waiting", "walking", "bed", "waiting", "soup", "eating", "complete" };

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

    void Start()
    {
        speed = 7.0f;
        targetPosition = transform.position;
        rbody = GetComponent<Rigidbody>();
        icon = Instantiate(iconPrefab, FindObjectOfType<Canvas>().transform);

        sicknessIconBackground = icon.transform.GetChild(0).GetComponent<Image>();
        sicknessIcon = icon.transform.GetChild(1).GetComponent<Image>();
        healingIcon = icon.transform.GetChild(2).GetComponent<Image>();
        healingOrderIcons = sickness.healingOrderIcons;
        //change based on patient type
        if (sickness.type == "Germ")
        {
            sicknessIconBackground.sprite = sickness.sicknessIconBackGround;
            sicknessIcon.sprite = sickness.sicknessIcon;
            healingIcon.sprite = healingOrderIcons[0];
        }

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
        //need to check patient state instead later
        if((targetPosition - transform.position).sqrMagnitude > 1.5f)
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
        print(rbody.velocity);
        print(direction);
        print(speed);
    }
    //set initial values
    private void setState()
    {
        actionsVal = 0;
        currState = actions[actionsVal];
        Debug.Log(currState);
    }

    //move to next state
    private void iterateState()
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
        folder = null;
    }

    public void folderPlaced(GameObject place)
    {
        assignedPlacement = place;
        targetPosition = assignedPlacement.transform.position - new Vector3(0,0,1.5f);
    }

    public void moveInQueue(Vector3 pos)
    {
        queuePosition = pos;
        targetPosition = queuePosition;
    }
}
