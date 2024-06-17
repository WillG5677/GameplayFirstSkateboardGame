using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // this should match the name of the walking animation bool in the animator in the unity editor
    private const string IS_WALKING = "IsWalking";

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    // private void Update() {
    //     animator.SetBool(IS_WALKING, player.IsWalking());
    // }
}
