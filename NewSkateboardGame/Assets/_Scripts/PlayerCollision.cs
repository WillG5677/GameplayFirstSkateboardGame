using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    void OnCollisionEnter (Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Wall") {
            // timeout player movement
        }

        else if (collisionInfo.collider.tag == "Destructable") {
            // add to chaos counter

            // logic for breaking up object
        }
    }
}
