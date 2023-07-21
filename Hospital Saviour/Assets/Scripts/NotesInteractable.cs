using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesInteractable : BaseInteractable
{
    public override void MainInteract(GameObject gameObject)
    {
        if(!CheckIfCarrying())
        {
            GetComponent<Collider>().enabled = false;
            transform.parent = gameObject.transform;
            changeObjectPos();
        }
    }

    private void changeObjectPos()
    {
        Vector3 newPos = new Vector3(0f, 0.5f, 0.85f);
        transform.localPosition = newPos;
        transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
    }
}
