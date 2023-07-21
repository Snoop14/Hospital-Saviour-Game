using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player1 : MonoBehaviour
{
    [SerializeField] float speed;

    private Vector3 movementVec;
    
    private Rigidbody rbody;

    private GameObject interactable;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
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
        if(interactable != null)
        {
            interactable.GetComponent<BaseInteractable>().MainInteract(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Interactable")
        {
            interactable = other.gameObject;
            other.GetComponent<BaseInteractable>().setPlayer(gameObject);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            interactable = null;
            other.GetComponent<BaseInteractable>().setPlayer(null);
        }
    }
}
