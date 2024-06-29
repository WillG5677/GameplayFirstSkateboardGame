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
        return Input.GetKey(KeyCode.Space);
    }

}
