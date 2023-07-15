using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    [SerializeField] float speed;
    private Player1_Input inputActions;
    private Rigidbody rbody;
    private Vector2 moveInput;

    private void Awake()
    {
        inputActions = new Player1_Input();
        rbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputActions.Player_Main.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player_Main.Disable();
    }

    void FixedUpdate()
    {
        moveInput = inputActions.Player_Main.Movement.ReadValue<Vector2>();
        rbody.velocity = new Vector3(moveInput.x, 0, moveInput.y) * speed;
    }
}
