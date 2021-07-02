using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRockLever : MonoBehaviour
{
    public bool rockInPlace;
    public Animator moveRockAni;
    public Animator leverAni;

    public void OpenDoor()
    {
        if (rockInPlace)
        {
            leverAni.SetTrigger("open");
            moveRockAni.SetTrigger("open");
        }
    }
}
