using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Germ1 : BaseInteractable
{
    //array to hold states of this patient type
    private string[] actions = { "waiting", "walking", "bed", "waiting", "soup", "complete" };

    //integer to hold current position in the state array
    private int actionsVal;

    //string to hold current state
    private string currState;


    // Start is called before the first frame update
    void Start()
    {
        //set initial values
        actionsVal = 0;
        currState = actions[actionsVal];
    }

    // Update is called once per frame
    //void Update()
    //{
        //debug log the surrent state of the patient
      //  Debug.Log(currState);

    //}

    public override void MainInteract(GameObject playerObject)
    {
        //add 1 to the current position
        actionsVal += 1;
        //shange the state
        currState = actions[actionsVal];
        //Debug log the state
        Debug.Log(currState);
    }
}
