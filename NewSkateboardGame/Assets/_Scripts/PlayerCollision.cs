using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float CHAOS_INCREMENT_AMOUNT = 10f;
    [SerializeField] private AudioClip breakingSound;
    void OnCollisionEnter (Collision collisionInfo) {

        if (collisionInfo.collider.tag == "Wall") {
            // timeout player movement
            StartCoroutine(playerMovement.DisableMovement(1f));
        }

        else if (collisionInfo.collider.tag == "Destructable") {
            // add to chaos counter
            ChaosManager.Instance.IncreaseChaos(CHAOS_INCREMENT_AMOUNT);

            // play sound

            // logic for breaking up object
        }
    }
}
