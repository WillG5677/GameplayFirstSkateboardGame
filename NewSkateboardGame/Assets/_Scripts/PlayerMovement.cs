using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Animator animator;
    Rigidbody rb;


    [Header("Movement")]
    private float currentSpeed;
    private bool canMove = true;
    [SerializeField] private float BASE_MOVE_SPEED = 200f;
    // [SerializeField] private float ACCELERATION = 7f;
    [SerializeField] private float MAX_SPEED = 50f;

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

    [SerializeField]
    private GameObject grindPrompt;
    private SplineAnimate splineAnimator;
    private readonly List<KeyValuePair<GrindEdge, Collider>> nearbyGrindEdges = new List<KeyValuePair<GrindEdge, Collider>>();
    private bool isGrinding = false;
    private GrindEdge bestGrindEdge;

    private void Start()
    {
        splineAnimator = GetComponent<SplineAnimate>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // animation
        animator.SetFloat("Speed", GetSpeed());
        animator.SetBool("isJumping", !grounded);

        if (isGrinding)
        {
            if (splineAnimator.NormalizedTime >= 0.98f)
            {
                EndGrind();
            }
        }
        else
        {
            bestGrindEdge = GetBestGrindEdge();
            grindPrompt.SetActive(bestGrindEdge != null);
        }

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
        if (canMove) {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        rotationInput = gameInput.GetRotationInput();
        forwardInput = gameInput.GetForwardInput();

        // when to jump
        if(gameInput.JumpInput() && readyToJump && grounded)
        {
            readyToJump = false;

            if (bestGrindEdge == null)
            {
                Jump();
                // animator.SetBool("isJumping", true);

                Invoke(nameof(ResetJump), JUMP_COOLDOWN);
            }
            else
            {
                Grind();
            }
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

    private void Grind()
    {
        if (bestGrindEdge == null)
        {
            return;
        }

        grindPrompt.SetActive(false);
        isGrinding = true;
        canMove = false;
        splineAnimator.Container = bestGrindEdge.GrindSpline;
        splineAnimator.Loop = bestGrindEdge.reverseSpline ? SplineAnimate.LoopMode.ReverseOnce : SplineAnimate.LoopMode.Once;
        //splineAnimator.MaxSpeed = Mathf.Max(10f, GetSpeed());
        splineAnimator.Play();
    }

    private void EndGrind()
    {
        if (!isGrinding)
        {
            return;
        }

        isGrinding = false;
        canMove = true;
        readyToJump = true;
        splineAnimator.Container = null;
        // Teleport rigidbody to new position
        rb.position = transform.position;
        rb.rotation = transform.rotation;
    }

    public float GetSpeed() {
        float speed = rb.velocity.magnitude;
        return speed;
    }

    public void BoostPlayer() {
        // TODO: when player steps on puddle, etc that boosts player speed. Timeout for when player can get above max speed
    }

    private GrindEdge GetBestGrindEdge()
    {
        GrindEdge bestGrindEdge = null;
        if (nearbyGrindEdges.Count == 0)
        {
            return bestGrindEdge;
        }

        float smallestAngle = float.MaxValue;
        foreach (KeyValuePair<GrindEdge, Collider> nearbyGrindEdge in nearbyGrindEdges)
        {
            float rightAlignAngle = Vector3.Angle(transform.right,
                Quaternion.Euler(0f, nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            Vector3 cross = Vector3.Cross(transform.right,
                Quaternion.Euler(0f, nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            if (cross.y < 0f)
            {
                rightAlignAngle *= -1f;
            }

            float leftAlignAngle = Vector3.Angle(transform.right,
                Quaternion.Euler(0f, -nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            cross = Vector3.Cross(transform.right,
                Quaternion.Euler(0f, -nearbyGrindEdge.Key.approachAngleRange / 2f, 0f) * nearbyGrindEdge.Key.approachVector);
            if (cross.y < 0f)
            {
                leftAlignAngle *= -1f;
            }

            float faceAngle = Vector3.Angle(transform.right, (nearbyGrindEdge.Key.attachPosition - transform.position).normalized);
            bool angleGood = faceAngle >= Mathf.Min(leftAlignAngle, rightAlignAngle) &&
                faceAngle <= Mathf.Max(leftAlignAngle, rightAlignAngle) &&
                faceAngle <= nearbyGrindEdge.Key.approachAngleRange / 2f;

            if (!angleGood)
            {
                continue;
            }

            if (faceAngle < smallestAngle)
            {
                smallestAngle = faceAngle;
                bestGrindEdge = nearbyGrindEdge.Key;
            }
        }
        return bestGrindEdge;
    }

    private void OnTriggerEnter(Collider other)
    {
        GrindEdge grindEdge = other.GetComponent<GrindEdge>();
        if (grindEdge == null)
        {
            return;
        }

        nearbyGrindEdges.Add(new KeyValuePair<GrindEdge, Collider>(grindEdge, other));
    }

    private void OnTriggerExit(Collider other)
    {
        GrindEdge grindEdge = other.GetComponent<GrindEdge>();
        if (grindEdge == null)
        {
            return;
        }

        nearbyGrindEdges.Remove(new KeyValuePair<GrindEdge, Collider>(grindEdge, other));
    }

    public IEnumerator DisableMovement(float seconds) {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }
}