using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player1 : MonoBehaviour
{
    [SerializeField] float speed;

    private Vector3 movementVec;

    
    private Rigidbody rbody;

    void Start()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
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

    }
}
