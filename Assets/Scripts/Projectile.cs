using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<RangedEnemy>())
            {
                other.gameObject.GetComponent<RangedEnemy>().TakeDamage(50f);
            }

            if (other.gameObject.GetComponent<MeleeEnemy>())
            {
                other.gameObject.GetComponent<MeleeEnemy>().TakeDamage(50f);
            }
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }
}
