using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        speed = 7.0f;
        targetPosition = transform.position;
        rbody = GetComponent<Rigidbody>();
        setState();
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
        targetPosition = assignedPlacement.transform.position - new Vector3(1.5f,0,0);
    }

    public void moveInQueue(Vector3 pos)
    {
        queuePosition = pos;
        targetPosition = queuePosition;
    }
}
