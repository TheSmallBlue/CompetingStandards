using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmoHelpers
{
    public static void DrawArrow(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(from, to);

        var arrowHeadStart = Vector3.Lerp(from, to, 0.5f);

        Gizmos.DrawLine(to, arrowHeadStart + Vector3.Cross((to - from).normalized, Vector3.up) * 0.5f);
        Gizmos.DrawLine(to, arrowHeadStart - Vector3.Cross((to - from).normalized, Vector3.up) * 0.5f);
    }
}
