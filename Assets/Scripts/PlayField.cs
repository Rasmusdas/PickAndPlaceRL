using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    public GameObject[] spawnPlates;

    public GameObject[] targetPlates;

    public GameObject target;

    public GameObject staticObjects;

    public GameObject basePlate;

    public Vector3 GetSpawnPoint()
    {
        GameObject randomSpawnPlate = spawnPlates[Random.Range(0, targetPlates.Length)];


        return GetPointOnPlate(randomSpawnPlate);
    }

    public Vector3 GetTargetPoint()
    {
        GameObject randomTargetPlate = targetPlates[Random.Range(0, targetPlates.Length)];

        return GetPointOnPlate(randomTargetPlate);
    }

    public Vector3 GetPointOnPlate(GameObject plate)
    {
        var bounds = plate.GetComponent<Renderer>().bounds;

        var pos = bounds.min + 2 * new Vector3(Random.value * bounds.extents.x, 0, Random.value * bounds.extents.z);

        return new Vector3(pos.x,transform.position.y + 0.5f,pos.z);
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public Bounds GetBaseBounds => basePlate.GetComponent<Renderer>().bounds;

    public void Augment()
    {
        RotateRandomly();
    }

    public void RotateRandomly()
    {
        staticObjects.transform.Rotate(0, Random.Range(0, 4) * 90, 0);
    }
}
