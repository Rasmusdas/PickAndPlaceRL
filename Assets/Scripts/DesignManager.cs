using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

public class DesignManager : MonoBehaviour
{
    int gridSize = 10;
    
    public Material floorMat;

    public List<GameObject> tiles = new List<GameObject>();
    private Camera cam;
    
    private int placementIndex = 1;
    private TileType[,] obstacleMap;

    PrefabManager prefabManager => PrefabManager.instance;

    private string pathName;

    Dictionary<int, TileType> indexToTileType = new Dictionary<int, TileType>()
    {
        {1 , TileType.Wall },
        {2 , TileType.Start },
        {3 , TileType.TargetFloor },
        {4 , TileType.Spike }
    };

    private void Start()
    {
        cam = Camera.main;

        if(!Directory.Exists("levels"))
        {
            Directory.CreateDirectory("levels");
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(25, 25, 20, 20), "-")) gridSize = Mathf.Max(0,gridSize-1);
        GUI.Label(new Rect(55, 25, 20, 20), gridSize.ToString());
        if (GUI.Button(new Rect(85, 25, 20, 20), "+")) gridSize++;

        if (GUI.Button(new Rect(25, 50, 80, 20), "Create Board")) CreateBaseBoard();

        if (GUI.Button(new Rect(25, 75, 80, 20), "Print Board")) Debug.Log(PrettyPrint());

        pathName = GUI.TextField(new Rect(25, 100, 115, 20), pathName);

        if (GUI.Button(new Rect(25, 125, 80, 20), "Save Board")) File.WriteAllText("levels/"+pathName,GetTextRepresentation());


    }

    private void CreateBaseBoard()
    {
        foreach(var v in tiles)
        {
            Destroy(v);
        }

        obstacleMap = new TileType[gridSize, gridSize];

        tiles = new List<GameObject>();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                var prim = GameObject.CreatePrimitive(PrimitiveType.Quad);

                prim.transform.position = new Vector3(i, 0, j);
                prim.transform.Rotate(90, 0, 0);

                tiles.Add(prim);

                prim.tag = "Floor";

                prim.GetComponent<Renderer>().material = floorMat;

                obstacleMap[i, j] = TileType.Empty;
            }
        }

        cam.orthographicSize = gridSize / 2.0f;

        cam.transform.position = gridSize % 2 == 0 ? new Vector3((gridSize-1) / 2.0f, gridSize, (gridSize-1) / 2.0f) : new Vector3((gridSize) / 2, gridSize, (gridSize) / 2); ;
        cam.transform.forward = Vector3.down;
    }

    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.CompareTag("Floor"))
            {
                if(Input.GetMouseButton(0))
                {
                    var placePrefab = prefabManager[indexToTileType[placementIndex]];
                    var placedObject = Instantiate(placePrefab,hit.collider.transform.position,placePrefab.transform.rotation);

                    placedObject.transform.position = hit.collider.transform.position + Vector3.up/2;

                    tiles.Add(placedObject);

                    var rounded = hit.transform.position.Round();

                    obstacleMap[rounded.x, rounded.z] = indexToTileType[placementIndex];
                }
            }

            if(!hit.collider.CompareTag("Floor"))
            {
                if (Input.GetMouseButton(1))
                {
                    tiles.Remove(hit.collider.transform.root.gameObject);

                    Destroy(hit.collider.transform.root.gameObject);
                    
                    var rounded = hit.transform.position.Round();

                    obstacleMap[rounded.x, rounded.z] = TileType.Empty;
                }
            }
        }

        int num = GetNumberInput();

        if (num != -1)
        {
            placementIndex = num;
        }
    }

    private int GetNumberInput()
    {
        if(Input.inputString != ""){
            bool isANumber = int.TryParse(Input.inputString, out var number);

            return number;
        }

        return -1;
    }

    private string PrettyPrint()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"{gridSize}\n");
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                sb.Append($";{obstacleMap[i, j]}");
            }

            sb.Append("\n");
        }

        return sb.ToString();
    }

    private string GetTextRepresentation()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"{gridSize}");
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                sb.Append($";{(int)obstacleMap[i, j]}");
            }
        }

        return sb.ToString();
    }
}
