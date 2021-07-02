using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject doorToOpen;
    public Animator doorAnimator;

    private void Start()
    {
        doorAnimator = doorToOpen.GetComponent<Animator>();
    }
}
