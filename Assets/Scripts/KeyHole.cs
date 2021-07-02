using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHole : MonoBehaviour
{
    public Transform keyPosition;
    public GameObject doorToOpen;
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        doorToOpen.SetActive(false);
    }
}
