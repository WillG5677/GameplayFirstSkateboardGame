using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float CHAOS_INCREMENT_AMOUNT = 10f;
    [SerializeField] private float DISABLED_MOVEMENT_TIME;
    [SerializeField] private float BOUNCE_MULTIPLIER;
    // Stop force controls how much the player gets launched from the wall, higher value is less launch
    [SerializeField] private float STOP_FORCE;
    [SerializeField] private float ROTATION_ANGLE;
    [SerializeField] private AudioClip breakingSound;

    private SpriteExplosion spriteExplosion;
    void OnCollisionEnter (Collision collisionInfo) {

        if (collisionInfo.collider.tag == "Wall") {
            float randomY = ROTATION_ANGLE*Random.Range(1,4);
            Vector3 currentVelocity = rb.velocity;
            Vector3 oppositeDirection = -currentVelocity.normalized;
            Vector3 launchForce = oppositeDirection * currentVelocity.magnitude*BOUNCE_MULTIPLIER;
            rb.AddForce(launchForce, ForceMode.Impulse);
            Quaternion randomRotation = Quaternion.Euler(0f, randomY, 0f);

            StartCoroutine(RotateTo(randomRotation));

            // timeout player movement after hitting a wall
            StartCoroutine(playerMovement.DisableMovement(DISABLED_MOVEMENT_TIME));
        }

        else if (collisionInfo.collider.tag == "Destructable") {
            // add to chaos counter
            ChaosManager.Instance.IncreaseChaos(CHAOS_INCREMENT_AMOUNT);

            // play sound

            // logic for breaking up object
            spriteExplosion = collisionInfo.gameObject.GetComponent<SpriteExplosion>();
            spriteExplosion.Explode();
        }
    }

    System.Collections.IEnumerator RotateTo(Quaternion targetRotation) {
        Quaternion initialRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < 1.5f)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed);
            elapsed += Time.deltaTime * STOP_FORCE / 360f;
            rb.AddForce(Vector3.right*BOUNCE_MULTIPLIER, ForceMode.Impulse);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

}
