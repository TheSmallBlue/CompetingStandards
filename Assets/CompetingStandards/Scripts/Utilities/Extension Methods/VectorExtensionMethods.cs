using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensionMethods
{

    #region Individual Axis Management

    /// <summary>
    /// Sets the value of an axis to another.
    /// </summary>
    /// <param name="vector"> The vector to modify </param>
    /// <param name="axis"> The axis to change </param>
    /// <param name="number"> The value to set in the axis </param>
    /// <returns></returns>
    public static Vector3 SetAxis(this Vector3 vector, Axis axis, float value)
    {
        if ((axis & Axis.X) == Axis.X) vector.x = value;
        if ((axis & Axis.Y) == Axis.Y) vector.y = value;
        if ((axis & Axis.Z) == Axis.Z) vector.z = value;

        return vector;
    }

    /// <summary>
    /// Adds a value to the value in a vector's axis.
    /// </summary>
    /// <param name="vector"> The vector to modify </param>
    /// <param name="axis"> The axis to change </param>
    /// <param name="number"> The value to add to the axis </param>
    /// <returns></returns>
    public static Vector3 AddToAxis(this Vector3 vector, Axis axis, float value)
    {
        if ((axis & Axis.X) == Axis.X) vector.x += value;
        if ((axis & Axis.Y) == Axis.Y) vector.y += value;
        if ((axis & Axis.Z) == Axis.Z) vector.z += value;

        return vector;
    }

    /// <summary>
    /// Sets the value of an axis to 0.
    /// </summary>
    /// <param name="vector"> The vector to modify </param>
    /// <param name="axis"> The axis to change </param>
    /// <returns></returns>
    public static Vector3 CollapseAxis(this Vector3 vector, Axis axis)
    {
        return vector.SetAxis(axis, 0);
    }

    #endregion

    /// <summary>
    /// Collapses the direction of a vector.
    /// </summary>
    /// <param name="source"> The vector to modify </param>
    /// <param name="direction"> The direction to nullify </param>
    /// <returns></returns>
    public static Vector3 CollapseDirection(this Vector3 source, Vector3 direction)
    {
        return source - (Vector3.Dot(source, direction) * direction);
    }
    //Vector3.Dot() It lets you get the magnitude of a component of a vector that is in a particular direction
}

public enum Axis
{
    X = 1,
    Y = 2,
    XY = 3,
    YX = 3,
    Z = 4,
    XZ = 5,
    ZX = 5,
    YZ = 6,
    ZY = 6,
    XYZ = 7
}




