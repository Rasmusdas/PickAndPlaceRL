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

    public static T GetRandomElement<T>(this T[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }

    public static T GetRandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
