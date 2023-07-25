using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Germ1 : BaseInteractable
{
    //array to hold states of this patient type
    private string[] actions = { "waiting", "walking", "bed", "waiting", "soup", "eating", "complete" };

    //integer to hold current position in the state array
    private int actionsVal;

    //string to hold current state
    private string currState;

    //bool to hold if carrying notes
    private bool withNotes = true;


    // Start is called before the first frame update
    void Start()
    {
        setState();
    }

    // Update is called once per frame
    //void Update()
    //{
        //debug log the surrent state of the patient
      //  Debug.Log(currState);

    //}

    public override void MainInteract(GameObject playerObject)
    {
       
        //prevent sate going out of range whilst testing (replace contents later with "walk out of scene" action & add score etc.)
        if (currState == "complete")
        {
            setState();
        }
        //carrying notes, with interaction needs to give up notes
        else if(withNotes == true)
        {
            //from https://docs.unity3d.com/ScriptReference/Transform.Find.html
            object notes = gameObject.transform.Find("Folder");

            //find notes
            if (notes != null  && playerObject.GetComponent<Player1>().isCarrying == false)
            {
                Debug.Log(notes);

                //give up notes
                // call MainInteract from NotesInteractable from the Folder child

                //notes.MainInteract(playerObject);
                //notes.GetComponent(MainInteract);
                //notes.GetComponent(NotesInteractable);
                gameObject.GetComponentInChildren<NotesInteractable>().MainInteract(playerObject);

                //change notes value
                withNotes = false;

                //iterate state
                iterateState(); 
            }

            
           
        }
        else
        {
            iterateState();
        }
        //Debug log the state
        //Debug.Log(currState);
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
}
