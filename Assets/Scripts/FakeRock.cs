using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeRock : MonoBehaviour
{
    public bool isFake;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Fall()
    {
        animator.SetTrigger("fall");
    }
}
