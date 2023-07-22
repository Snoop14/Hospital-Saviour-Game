using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteractable : BaseInteractable
{
    public override void MainInteract(GameObject playerObject)
    {
        if (playerObject.GetComponent<Player1>().isCarrying)
        {
            NotesDropOff();
        }
    }

    private void NotesDropOff()
    {
        foreach(Transform tempTrans in player.GetComponentsInChildren<Transform>())
        {
            if(tempTrans.name == "Folder")
            {
                tempTrans.parent = transform;
            }
        }
    }

    private void NotesPickUp()
    {

    }

    private void NPCInteract()
    {

    }
}
