using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    Level level;
   
    PrefabManager prefabManager => PrefabManager.instance;  

    string[] levelPaths;

    public static LevelManager instance;

    private void Awake()
    {
        instance = this;
        if (!Directory.Exists("levels"))
        {
            Directory.CreateDirectory("levels");
        }
        levelPaths = Directory.GetFiles("levels");
    }

    public (GameObject,Level) CreateRandomLevel()
    {
        string levelPath = levelPaths.GetRandomElement();

        var newLevel = Level.LoadFromFile(levelPath);

        return (CreateLevel(newLevel),newLevel);
    }

    public GameObject CreateLevel(Level level)
    {
        GameObject levelParent = new GameObject("Level");

        for (int i = 0; i < level.Size; i++)
        {
            for(int j = 0; j < level.Size; j++)
            {
                TileType levelIdx = level[i, j];

                if(levelIdx >= 0)
                {
                    Instantiate(prefabManager[levelIdx],new Vector3(i, 0, j), prefabManager[levelIdx].transform.rotation, levelParent.transform);
                }
            }
        }

        var floorInst = Instantiate(prefabManager[TileType.Empty], new Vector3(0,0,0), prefabManager[TileType.Empty].transform.rotation, levelParent.transform);

        floorInst.transform.position = new Vector3((level.Size - 1) / 2.0f, -0.5f, (level.Size - 1) / 2.0f);
        floorInst.transform.localScale = new Vector3(level.Size, level.Size,1);


        return levelParent;
    }
}


public class Level
{
    TileType[,] levelGrid;

    public int Size => levelGrid.GetLength(0);

    public TileType this[int x, int y]
    {
        get { return levelGrid[x, y]; }
        set { levelGrid[x, y] = value; }
    }

    public Level(TileType[,] grid)
    {
        this.levelGrid = grid;
    }

    public static Level LoadFromFile(string path)
    {
        return LoadFromString(File.ReadAllText(path));
    }

    public static Level LoadFromString(string str)
    {
        var splitString = str.Split(";");

        int size = int.Parse(splitString[0]);

        TileType[,] levelGrid = new TileType[size, size];

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                levelGrid[i,j] = (TileType)int.Parse(splitString[1+i*size+j]);

                
            }
        }

        return new Level(levelGrid);
    }

    public Level ToSimpleRepresentation()
    {
        TileType[,] levelClone = (TileType[,])levelGrid.Clone();

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (levelClone[i, j] != TileType.Empty && levelClone[i, j] != TileType.Agent && levelClone[i, j] != TileType.Wall)
                {
                    levelClone[i, j] = TileType.Empty;
                }
            }
        }

        return new Level(levelClone);
    }
}

public enum TileType
{
    Empty,
    Wall,
    Start,
    TargetFloor,
    Spike,
    Target,
    Agent
    
}