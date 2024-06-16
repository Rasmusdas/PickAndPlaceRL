using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3Int Round(this Vector3 vector3)
    {
        return new Vector3Int(
            (int)Mathf.Round(vector3.x),
            (int)Mathf.Round(vector3.y),
            (int)Mathf.Round(vector3.z));
    }
}
