using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableRock : MonoBehaviour
{
    public MoveRockLever lever;
    public Animator ani;
    public Animator moveRockAni;
    public Transform rayCastPoint;
    public LayerMask rockMask;
    public void MoveRock()
    {
        Debug.Log("rockmoved");
        RaycastHit hit;
        
        if (!Physics.Raycast(rayCastPoint.position, rayCastPoint.forward, out hit, 2f, rockMask))
        {
            ani.SetTrigger("forward1");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RockEnter"))
        {
            lever.rockInPlace = true;
        }
    }
}
