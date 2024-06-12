using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    // public Rigidbody rb;

    // public bool isPlaying = true;
    public float forwardForce = 10f;
    public float turnSpeed = 75f;
    public float forwardInput;
    public float sidewaysInput;

    // Start is called before the first frame update
    void Start()
    {

    }
    void Update() {
        // read values from keyboard and store in computer
        forwardInput = Input.GetAxis("Vertical");
        sidewaysInput = Input.GetAxis("Horizontal");
    }

    // Update is called once per frame, used for physics processes
    void FixedUpdate()
    {
        // add a consistent forward force when game is started
        // if ( isPlaying ) {
            transform.Translate(Vector3.forward * forwardForce * Time.deltaTime);
        // }

        transform.Translate(Vector3.forward * forwardInput * Time.deltaTime);
        transform.Rotate(Vector3.up * sidewaysInput * turnSpeed * Time.deltaTime);


        // // player inputs
        // if ( Input.GetKey("a") ) {
        //     rb.AddForce(-inputForce * Time.deltaTime, 0, 0);
        // }
        // if ( Input.GetKey("d") ) {
        //     rb.AddForce(inputForce * Time.deltaTime, 0, 0);
        // }
        // // w can speed up player and s can slow down player
        // if ( Input.GetKey("w") ) {
        //     rb.AddForce(0, 0, inputForce * Time.deltaTime);
        // }
        // // s is not slowing down the player
        // if ( Input.GetKey("s") ) {
        //     rb.AddForce(0, 0, -inputForce * Time.deltaTime);
        // }
        // // jump
        // if ( Input.GetKey(" ") ) {
        //     rb.AddForce(0, jumpForce * Time.deltaTime, 0);
        // }
    }
}
