using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    [SerializeField] GameObject floor;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject targetFloor;
    [SerializeField] GameObject target;
    [SerializeField] GameObject startFloor;
    [SerializeField] GameObject spike;
    [SerializeField] GameObject agent;

    public Dictionary<TileType, GameObject> tileToPrefab;

    public GameObject this[TileType type]
    {
        get { return tileToPrefab[type]; }

    }

    private void Awake()
    {
        instance = this;

        tileToPrefab = new Dictionary<TileType, GameObject>() {
            {TileType.Empty, floor},
            {TileType.Agent, agent},
            {TileType.Wall, wall},
            {TileType.Start, startFloor},
            {TileType.Target, target},
            {TileType.TargetFloor, targetFloor},
            {TileType.Spike, spike},
        };
    }




}
