using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityModel
{
    private bool[,] visited;
    private Vector3 min;
    private int cellSize;
    private int gridSize;
    private Vector3 offset;
    public DensityModel(Vector3 min, int gridSize, int cellSize, Vector3 offset)
    {
        this.min = min;
        this.cellSize = cellSize;
        this.gridSize = gridSize;
        this.offset = offset;
        visited = new bool[gridSize, gridSize];
    }

    public void AddState(Vector3 state)
    {
        var roundState = ((state - min) / cellSize).Round();

        roundState.Clamp(Vector3Int.zero, Vector3Int.one * (gridSize - 1));

        visited[roundState.x, roundState.z] = true;
    }

    public bool GetVisited(Vector3 state)
    {
        var roundState = ((state - min) / cellSize).Round();

        roundState.Clamp(Vector3Int.zero, Vector3Int.one * (gridSize - 1));

        return visited[roundState.x, roundState.z];
    }

    public List<float> FlattenHeatMap()
    {
        List<float> result = new List<float>();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                result.Add(visited[i, j] ? 1 : 0);
            }
        }

        return result;
    }
}

