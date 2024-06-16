using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DesignManager : MonoBehaviour
{
    int gridSize = 10;

    public Material floorMat;

    public List<GameObject> tiles = new List<GameObject>();
    private void OnGUI()
    {
        if (GUI.Button(new Rect(25, 25, 20, 20), "-")) gridSize = Mathf.Max(0,gridSize-1);
        GUI.Label(new Rect(55, 25, 20, 20), gridSize.ToString());
        if (GUI.Button(new Rect(85, 25, 20, 20), "+")) gridSize++;

        if (GUI.Button(new Rect(25, 50, 80, 20), "Create Board")) CreateBaseBoard();
    }


    private void CreateBaseBoard()
    {
        foreach(var v in tiles)
        {
            Destroy(v);
        }

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
            }
        }

        var cam = Camera.main;

        cam.orthographicSize = gridSize / 2.0f;

        cam.transform.position = gridSize % 2 == 0 ? new Vector3((gridSize-1) / 2.0f, gridSize, (gridSize-1) / 2.0f) : new Vector3((gridSize) / 2, gridSize, (gridSize) / 2); ;
        cam.transform.forward = Vector3.down;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.tag == "Floor")
            {
                if(Input.GetMouseButton(0))
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    cube.transform.position = hit.collider.transform.position + Vector3.up/2;

                    tiles.Add(cube);
                }
            }

            if(hit.collider.tag != "Floor")
            {
                if (Input.GetMouseButton(1))
                {
                    tiles.Remove(hit.collider.gameObject);

                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}
