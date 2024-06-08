using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public PlayerMovement movement;

    void OnCollisionEnter (Collision collisionInfo) {
        Debug.Log(collisionInfo.collider.tag);
        
        // stopping player control of movement when a collidable is hit
        if (collisionInfo.collider.tag == "Collidable") {
            movement.enabled =  false;
        }
    }

}
