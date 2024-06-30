using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{


    public float GetForwardInput() {
        // not sure if this should be .GetAxis vs .GetAxisRaw
        float rotationInput = Input.GetAxisRaw("Horizontal");

        return rotationInput;
    }

    public float GetRotationInput() {
        float forwardInput = Input.GetAxisRaw("Vertical");

        return forwardInput;
    }

    public bool JumpInput() {
        return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button14);
    }

    public bool GrindInput()
    {
        return Input.GetKeyDown(KeyCode.F) || Input.GetKey(KeyCode.Joystick1Button13);
    }
}
