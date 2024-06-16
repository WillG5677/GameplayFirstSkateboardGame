using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.MeshOperations;

public class PlayerMovement : MonoBehaviour
{
    // private bool isPlaying = true;
    [Header("Speed")]
    [SerializeField] private float FORWARD_FORCE = 10f;
    // [SerializeField] private float LEVEL_ONE_SPEED = 10f;
    // [SerializeField] private float LEVEL_TWO_SPEED = 50f;
    // [SerializeField] private float LEVEL_THREE_SPEED = 90f;
    // [SerializeField] private float MAX_SPEED = 100f;
    [SerializeField] private float TURN_SPEED = 75f;


    
    [Header("Movement")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Rigidbody rb;

    // i just want to see these in the editor when the game is running
    [SerializeField] private float moveSpeed;
    [SerializeField] private float forwardInput;
    [SerializeField] private float rotationInput;

    private Transform orientation;
    private Vector3 moveDirection;

    [Header("Drag")]
    [SerializeField] private const float GROUND_DRAG = 1f;

    [Header("Jump")]
    [SerializeField] private const float JUMP_FORCE = 5f;
    [SerializeField] private const float JUMP_COOLDOWN = .25f;
    [SerializeField] private const float AIR_MULTIPLIER = .4f;
    private bool readyToJump;

    // ground check variables for checking whether or not to apply drag
    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    // [Header("Animations")]
    // private bool isWalking;

    // Start is called before the first frame update
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        // rb.freezeRotation = true;
    }
    private void Update() {

        // checking if different levels of speed reached/ max speed
        SpeedControl();

        // ### DRAG CODE ###
        // checking if player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2F, whatIsGround);

        // handle drag
        if (grounded)
            rb.drag = GROUND_DRAG;
        else
            rb.drag = 0;
    }

    // Update is called once per frame, used for physics processes
    void FixedUpdate()
    {
        // add a consistent forward force when game is started
        // if ( isPlaying ) {
            MovePlayer();
        // }

        // maybe this should also be in MovePlayer()
        transform.Translate(Vector3.forward * forwardInput * Time.deltaTime);
        transform.Rotate(Vector3.up * rotationInput * TURN_SPEED * Time.deltaTime);

        // isWalking = forwardForce != 0;

    }

    // public bool IsWalking() {
    //     return isWalking;
    // }

    private void HandleInputs() {
        // getting the inputs from the GameInput script
        rotationInput = gameInput.GetRotationInput();
        forwardInput = gameInput.GetForwardInput();

        // ### JUMP CODE ###
        // TODO: need to link jump input from GameInput script
        // bool jumpKeyPressed = gameInput.JumpInput();
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded) {
            readyToJump = false;
            jump();
            Invoke(nameof(ResetJump), JUMP_COOLDOWN);
        }
    }

    private void MovePlayer() {
        // calculate what is "forward"
        moveDirection = orientation.right * rotationInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * FORWARD_FORCE, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized *moveSpeed* FORWARD_FORCE * AIR_MULTIPLIER, ForceMode.Force);
    }

    private void SpeedControl() {
        // TODO
    }

    private void jump() {
        //reset y velocity, so you will always jump the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.x);
        // jump logic
        rb.AddForce(transform.up * JUMP_FORCE, ForceMode.Impulse);
    }

    private void ResetJump() {
        readyToJump = true;
    }
}
