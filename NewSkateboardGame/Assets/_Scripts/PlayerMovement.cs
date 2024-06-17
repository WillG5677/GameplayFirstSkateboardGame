using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerMovementTutorial : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameInput gameInput;
    Rigidbody rb;


    [Header("Movement")]

    [SerializeField] private float BASE_MOVE_SPEED = 10f;
    [SerializeField] private float MOVE_SPEED = 7f;

    [SerializeField] private float GROUND_DRAG = 5f;

    [SerializeField] private float JUMP_FORCE = 12f;
    [SerializeField] private float JUMP_COOLDOWN = .25f;
    [SerializeField] private float AIR_MULTIPLIER = .4f;
    bool readyToJump;

    [HideInInspector] [SerializeField] private float walkSpeed;
    [HideInInspector] [SerializeField] private float sprintSpeed;


    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    [SerializeField] private Transform orientation;

    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;

    [SerializeField] private Vector3 moveDirection;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded) {
            rb.drag = GROUND_DRAG;
            rb.angularDrag = 1;
        }
        else {
            rb.drag = 0;
            rb.angularDrag = 5;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = gameInput.GetRotationInput();
        verticalInput = gameInput.GetForwardInput();

        // when to jump
        if(gameInput.JumpInput() && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JUMP_COOLDOWN);
        }
    }

    private void MovePlayer()
    {
        // turn
        rb.AddTorque(rb.transform.up * horizontalInput * 200f * Time.deltaTime, ForceMode.Acceleration);

        // move forward
        // on ground
        if(grounded)
            rb.AddForce(rb.transform.forward * MOVE_SPEED * BASE_MOVE_SPEED, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(rb.transform.forward * MOVE_SPEED * BASE_MOVE_SPEED * AIR_MULTIPLIER, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > MOVE_SPEED)
        {
            Vector3 limitedVel = flatVel.normalized * MOVE_SPEED;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * JUMP_FORCE, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}