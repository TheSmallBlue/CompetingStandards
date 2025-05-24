using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorHelpers
{
    /// <summary>
    /// Takes an angle and returns a forward vector facing that angle.
    /// </summary>
    /// <param name="angle"> The angle, from -180 to 180 </param>
    /// <returns></returns>
    public static Vector3 AngleToForward(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    /// <summary>
    /// Returns the progress of point "value" between "a" and "b".
    /// </summary>
    /// <param name="a">From</param>
    /// <param name="b">To</param>
    /// <param name="value"> Point to calculate </param>
    /// <returns></returns>
    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
}
