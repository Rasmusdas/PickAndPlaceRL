using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using UnityEngine;

public class DesignManager : MonoBehaviour
{
    int gridSize = 10;
    
    public Material floorMat;

    public List<GameObject> tiles = new List<GameObject>();
    private Camera cam;

    public GameObject[] objectsToPlace;
    
    private int placementIndex = 0;
    private int[,] obstacleMap;
    
    
    

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(25, 25, 20, 20), "-")) gridSize = Mathf.Max(0,gridSize-1);
        GUI.Label(new Rect(55, 25, 20, 20), gridSize.ToString());
        if (GUI.Button(new Rect(85, 25, 20, 20), "+")) gridSize++;

        if (GUI.Button(new Rect(25, 50, 80, 20), "Create Board")) CreateBaseBoard();
        
        if (GUI.Button(new Rect(25, 75, 80, 20), "Print Board")) Debug.Log(GetMapStringFormat());
    }

    private void CreateBaseBoard()
    {
        foreach(var v in tiles)
        {
            Destroy(v);
        }

        obstacleMap = new int[gridSize, gridSize];

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

                obstacleMap[i, j] = -1;
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
                    var placePrefab = objectsToPlace[placementIndex];
                    var placedObject = Instantiate(placePrefab,hit.collider.transform.position,placePrefab.transform.rotation);

                    placedObject.transform.position = hit.collider.transform.position + Vector3.up/2;

                    tiles.Add(placedObject);

                    var rounded = hit.transform.position.Round();

                    obstacleMap[rounded.x, rounded.z] = placementIndex;
                }
            }

            if(!hit.collider.CompareTag("Floor"))
            {
                if (Input.GetMouseButton(1))
                {
                    tiles.Remove(hit.collider.transform.root.gameObject);

                    Destroy(hit.collider.transform.root.gameObject);
                    
                    var rounded = hit.transform.position.Round();

                    obstacleMap[rounded.x, rounded.z] = -1;
                }
            }
        }

        int num = GetNumberInput();

        if (num != -1)
        {
            placementIndex = Mathf.Min(num ,objectsToPlace.Length)-1;
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

    private string GetMapStringFormat()
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
}
