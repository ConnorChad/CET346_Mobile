#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RangedEnemy))]
public class EnemyFOV : Editor
{
    private void OnSceneGUI()
    {
       RangedEnemy fov = (RangedEnemy)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRange);
        Vector3 viewAngleA = fov.dirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.dirFromAngle(+fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRange);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRange);
    }
}
#endif