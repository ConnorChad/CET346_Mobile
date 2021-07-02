using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public MeleeSpawner[] meleeSpawners;

    public void Spawn()
    {
        for (int i = 0; i < meleeSpawners.Length; i++)
        {
            meleeSpawners[i].SpawnEnemy();
        }

        Destroy(gameObject);
    }
    
}
