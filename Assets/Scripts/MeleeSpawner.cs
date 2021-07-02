using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public GameObject vfx;
    public void SpawnEnemy()
    {
        Instantiate(vfx, transform.position, transform.rotation);
        Instantiate(enemyToSpawn, transform.position, transform.rotation);
    }
    
}
