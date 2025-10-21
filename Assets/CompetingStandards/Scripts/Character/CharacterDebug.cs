using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompetingStandards
{
    // -- Debug --

    public partial class Character : MonoBehaviour
    {
        // ---

        Dictionary<string, DebugVectorInfo> vectorsToDebug = new();

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

                Gizmos.color = vectorsByName[i].color;
                GizmoHelpers.DrawArrow(vectorsByName[i].position, vectorsByName[i].position + vectorsByName[i].direction);
            }
        }

        public void DebugVector(string name, DebugVectorInfo vector)
        {
            if (vectorsToDebug.ContainsKey(name))
            {
                vectorsToDebug[name] = vector;
                return;
            }

            vectorsToDebug.Add(name, vector);
        }

        public struct DebugVectorInfo
        {
            public Vector3 position;
            public Vector3 direction;
            public Color color;
        }
    }
}

