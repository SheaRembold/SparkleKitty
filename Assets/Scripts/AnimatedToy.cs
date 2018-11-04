using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedToy : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HitToy()
    {
        animator.SetTrigger("Hit");
    }
}