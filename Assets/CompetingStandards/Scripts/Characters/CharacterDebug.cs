using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Character : MonoBehaviour
{
    // ---

    Dictionary<string, Tuple<Vector3, Color>> vectorsToDebug = new();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * _groundSnappingFloorHeight, new Vector3(1f, 0f, 1f));

        Gizmos.color = Color.magenta;
        GizmoHelpers.DrawArrow(transform.position, transform.position + RB.velocity);

       // Vectors
       var vectorsByName = vectorsToDebug.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        for (int i = 0; i < vectorsByName.Count; i++)
        {
            float progress = i / vectorsByName.Count;

            Gizmos.color = vectorsByName[i].Item2;
            GizmoHelpers.DrawArrow(transform.position, transform.position + vectorsByName[i].Item1);
        }
    }

    public void DebugVector(string name, Tuple<Vector3, Color> vector)
    {
        if(vectorsToDebug.ContainsKey(name))
        {
            vectorsToDebug[name] = vector;
            return;
        }

        vectorsToDebug.Add(name, vector);
    }
}
