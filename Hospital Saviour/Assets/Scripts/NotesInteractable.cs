using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesInteractable : BaseInteractable
{
    public override void MainInteract(GameObject playerObject)
    {
        if(!playerObject.GetComponent<Player1>().isCarrying)
        {
            playerObject.GetComponent<Player1>().isCarrying = true;
            GetComponent<Collider>().enabled = false; //turns off the folders collider
            transform.parent = playerObject.transform; //changes the parent of folder to the player
            changeObjectPos();
        }
    }

    private void changeObjectPos()
    {
        transform.localPosition = new Vector3(0f, 0.5f, 0.85f);
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f); //resets rotation
    }
}
