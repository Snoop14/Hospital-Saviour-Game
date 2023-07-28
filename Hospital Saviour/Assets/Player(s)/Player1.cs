using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player1 : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 movementVec;
    
    private Rigidbody rbody;

    [SerializeField]
    GameObject gameManager;
    GameManager manager;

    public bool isCarrying = false;
    string itemType = "";
    GameObject item = null;

    List<GameObject> collidingObjects;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
        collidingObjects = new List<GameObject>();
        manager = gameManager.GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        //Gets player to look in direction of movement
        transform.LookAt(transform.position + movementVec, new Vector3(0, 1, 0));
        rbody.velocity = movementVec * speed; 
    }

    public void OnMove(InputValue input)
    {
        Vector2 xyInput = input.Get<Vector2>();
        movementVec = new Vector3(xyInput.x, 0, xyInput.y);
    }

    public void OnInteract()
    {
        if (isCarrying)
        {
            if(itemType == "Folder")
            {
                foreach (GameObject go in collidingObjects)
                {
                    if (manager.objectList.Contains(go))
                    {
                        Bed b = go.GetComponent<Bed>();
                        if (b && b.isActive && b.isInteractable && !b.hasFolder && !b.isOccupied)
                        {
                            Folder f = item.GetComponent<Folder>();
                            f.transferTo(go);
                            f.changePosToBed();
                            b.folderDropOff(item);
                            f.patientOwner.GetComponent<Patient>().folderPlaced(go);
                            manager.removeFromQueue(f.patientOwner);
                            isCarrying = false;
                            item = null;
                            itemType = "";

                            break;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (GameObject go in collidingObjects)
            {
                if (manager.objectList.Contains(go))
                {
                    Patient p = go.GetComponent<Patient>();
                    print(p);
                    if (p && p.isInteractable)
                    {
                        if (p.isHoldingFolder)
                        {
                            isCarrying = true;
                            item = p.folder;
                            itemType = "Folder";
                            p.releaseFolder();
                            Folder i = item.GetComponent<Folder>();
                            i.transferTo(gameObject);
                            i.changePosToPlayer();

                            break;
                        }
                    }
                }
            }
        }
    }

    //Called when the player enters in range of another object
    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            collidingObjects.Add(other.gameObject);
            print(other.gameObject.name + " entered collider");
        }
    }

    //Called when the player exits range of another object
    private void OnTriggerExit(Collider other)
    {
        if (collidingObjects.Contains(other.gameObject))
        {
            collidingObjects.Remove(other.gameObject);
            print(other.gameObject.name + " left collider");
        }
    }
}
