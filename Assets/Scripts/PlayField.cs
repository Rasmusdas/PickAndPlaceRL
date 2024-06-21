using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Sentis.Layers;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    List<Vector3> spawnPoints;
    List<Vector3> targetPoints;
    GameObject target;
    GameObject staticObjects;
    GameObject basePlate;
    Level level;

    PrefabManager prefabManager => PrefabManager.instance;

    PickupAgent agent;

    Vector3Int lastPlayerPos = -Vector3Int.one;

    GameObject[,] heatMapTiles;
    GameObject[,] levelMapTiles;

    Dictionary<TileType, Color> tileToColor = new Dictionary<TileType, Color>()
    {
        {TileType.Empty, Color.black},
        {TileType.Wall, Color.white},
        {TileType.Agent, Color.green},
        {TileType.Target, Color.red},
        {TileType.Spike, Color.blue}
    };


    DensityModel densityModel;

    public void SetAgent(PickupAgent agent)
    {
        this.agent = agent;
        agent.agentStatsUI.transform.position = transform.position + new Vector3(level.Size + 1,0, level.Size + 1);
    }

    public void LoadRandomLevel()
    {
        spawnPoints = new List<Vector3>();
        targetPoints = new List<Vector3>();
        var (levelGb, lvl) = LevelManager.instance.CreateRandomLevel();

        for (int x = 0; x < lvl.Size; x++)
        {
            for (int z = 0; z < lvl.Size; z++)
            {
                if(lvl[x, z] == TileType.Start)
                {
                    spawnPoints.Add(transform.position + new Vector3(x, 0, z));
                }
                if (lvl[x,z] == TileType.TargetFloor)
                {
                    targetPoints.Add(transform.position + new Vector3(x, 0, z));
                }
            }
        }

        target = Instantiate(prefabManager[TileType.Target]);
        level = lvl.ToSimpleRepresentation();
        CreateLevelMap(level);
        CreateHeatMap();
        densityModel = new DensityModel(transform.position,level.Size,1);
    }

    public void UpdatePlayerPosition(Vector3 pos)
    {
        var roundPos = pos.Round();
        if (lastPlayerPos != -Vector3Int.one)
        {
            level[lastPlayerPos.x, lastPlayerPos.z] = TileType.Empty;
            level[roundPos.x, roundPos.z] = TileType.Agent;
            densityModel.AddState(pos);
        }
        lastPlayerPos = roundPos;
    }

    private void Update()
    {
        UpdateVisualMaps();
    }

    public void UpdateVisualMaps()
    {
        UpdateHeatMap();
        UpdateLevelMap();
    }

    public Vector3 GetSpawnPoint()
    {


        return spawnPoints.GetRandomElement();
    }

    public Vector3 GetTargetPoint()
    {
        return targetPoints.GetRandomElement();
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public void Augment()
    {
        RotateRandomly();
    }

    public void RotateRandomly()
    {
        staticObjects.transform.Rotate(0, Random.Range(0, 4) * 90, 0);
    }

    private void CreateLevelMap(Level level)
    {
        GameObject map = new GameObject();
        levelMapTiles = new GameObject[level.Size, level.Size];
        for (int x = 0; x < level.Size; x++)
        {
            for (int z = 0; z < level.Size; z++)
            {
                Vector3 position = transform.position + new Vector3(x, 0, z);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.parent = map.transform;
                tile.transform.position = position;
                tile.transform.rotation = Quaternion.Euler(90, 0, 0); // Rotate to lie flat on the ground
                tile.GetComponent<Renderer>().material.color = tileToColor[level[x, z]];
                levelMapTiles[x, z] = tile;
            }
        }

        map.transform.position = new Vector3(level.Size + 1, 0, 0);
    }

    private void UpdateLevelMap()
    {
        for (int x = 0; x < level.Size; x++)
        {
            for (int z = 0; z < level.Size; z++)
            {
                levelMapTiles[x, z].GetComponent<Renderer>().material.color = tileToColor[level[x, z]];
            }
        }
    }


    private void CreateHeatMap()
    {
        GameObject map = new GameObject();
        heatMapTiles = new GameObject[level.Size, level.Size];
        for (int x = 0; x < level.Size; x++)
        {
            for (int z = 0; z < level.Size; z++)
            {
                Vector3 position = transform.position + new Vector3(x, 0, level.Size + 1 + z);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.parent = map.transform;
                tile.transform.position = position;
                tile.transform.rotation = Quaternion.Euler(90, 0, 0);
                heatMapTiles[x, z] = tile;
            }
        }
    }

    private void UpdateHeatMap()
    {
        for (int x = 0; x < level.Size; x++)
        {
            for (int z = 0; z < level.Size; z++)
            {
                Vector3 position = new Vector3(transform.position.x + x, 0, transform.position.z + z);
                bool density = densityModel.GetVisited(position);
                Color color = Color.Lerp(Color.red, Color.green, density ? 1 : 0);
                heatMapTiles[x, z].GetComponent<Renderer>().material.color = color;
            }
        }
    }
}
