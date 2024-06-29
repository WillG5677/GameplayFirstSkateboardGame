using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.PlasticSCM.Editor.WebApi;

public class PlayerMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Animator animator;
    Rigidbody rb;


    [Header("Movement")]
    private float currentSpeed;
    [SerializeField] private float BASE_MOVE_SPEED = 50f;
    // [SerializeField] private float ACCELERATION = 7f;
    [SerializeField] private float MAX_SPEED = 20f;

    [SerializeField] private float GROUND_DRAG = 5f;

    [SerializeField] private float JUMP_FORCE = 12f;
    [SerializeField] private float JUMP_COOLDOWN = .25f;
    [SerializeField] private float AIR_MULTIPLIER = .4f;
    [SerializeField] private float ROTATION_FORCE = 300f;
    [SerializeField] private float FORWARD_FORCE = .3f;
    bool readyToJump;

    [HideInInspector] [SerializeField] private float walkSpeed;
    [HideInInspector] [SerializeField] private float sprintSpeed;


    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    [SerializeField] private float rotationInput;
    [SerializeField] private float forwardInput;

    [SerializeField] private Vector3 moveDirection;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // animation
        animator.SetFloat("Speed", GetSpeed());
        animator.SetBool("isJumping", !grounded);

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
        rotationInput = gameInput.GetRotationInput();
        forwardInput = gameInput.GetForwardInput();

        // when to jump
        if(gameInput.JumpInput() && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            // animator.SetBool("isJumping", true);

            Invoke(nameof(ResetJump), JUMP_COOLDOWN);
        }
    }

    private void MovePlayer()
    {
        // turn
        rb.AddTorque(rb.transform.up * -rotationInput * ROTATION_FORCE * Time.deltaTime, ForceMode.Acceleration);

        // move forward
        // on ground
        if(grounded)
            rb.AddForce(rb.transform.right * BASE_MOVE_SPEED, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(rb.transform.right * BASE_MOVE_SPEED * AIR_MULTIPLIER, ForceMode.Force);

        // add forward force
            rb.AddForce(rb.transform.right * BASE_MOVE_SPEED * forwardInput * FORWARD_FORCE, ForceMode.Force);
        
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > MAX_SPEED)
        {
            Vector3 limitedVel = flatVel.normalized * MAX_SPEED;
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

    public float GetSpeed() {
        float speed = rb.velocity.magnitude;
        return speed;
    }

    public void BoostPlayer() {
        // TODO: when player steps on puddle, etc that boosts player speed. Timeout for when player can get above max speed
    }
}