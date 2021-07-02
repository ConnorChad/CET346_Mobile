using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PressurePad"))
        {
            other.GetComponent<PressurePlate>().doorAnimator.SetTrigger("open");
        }
    }
}
