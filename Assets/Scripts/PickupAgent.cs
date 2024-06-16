using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;
using System;
using Unity.Sentis.Layers;
using System.Runtime.CompilerServices;

public class PickupAgent : Agent
{
    public RenderTexture orgRT;
    public GameObject target;
    public float spawnDistance;
    public AgentStatsUI agentStatsUI;
    public Camera cam;
    public RenderTextureSensor rtSensor;
    CharacterController _cc;
    public float movementSpeed;
    System.Random rnd = new System.Random();

    public LayerMask targetMask;

    Vector3 warpTo = Vector3.zero;

    Vector3 min;

    public PlayField pF;

    Vector3 prevPos;

    private DensityModel densityModel;
    private const float densityBandwidth = 1.0f;

    private int gridSize = 10;
    private int cellSize = 2;
    private GameObject[,] heatMapTiles;


    public override void Initialize()
    {
        
    }

    void Awake()
    {
        base.Awake();
        target = pF.GetTarget();
        min = pF.GetBaseBounds.min;
        CreateHeatMap();

        
    }

    void SetupPositions()
    {
        Vector3 startPoint = pF.GetSpawnPoint();
        
        Vector3 targetPoint = pF.GetTargetPoint();

        densityModel = new DensityModel(min.Round(),gridSize,cellSize,Vector3.zero);
        PlaceAgent(startPoint);
        target.transform.position = targetPoint;
        SetReward(0);
    }

    void Start()
    {
        SetupPositions();
        _cc = GetComponent<CharacterController>();
        prevPos = transform.position;
    }


    private void LateUpdate()
    {
        if(warpTo != Vector3.zero)
        {
            transform.position = warpTo;
            warpTo = Vector3.zero;
            SetReward(0);
        }
    }

    //private void Update()
    //{
    //    SetupPositions();
    //}

    public override void OnEpisodeBegin()
    {
        pF.Augment();
        SetupPositions();
        prevPos = transform.position;
    }

    public void PlaceAgent(Vector3 pos)
    {
        warpTo = pos;
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        Vector3 currentState = transform.position;
        var disAction = actions.DiscreteActions[0];
        Vector3 action = Vector3.zero;
        switch(disAction)
        {
            case 0:
                break;
            case 1:
                action = Vector3.right;
                break;
            case 2:
                action = Vector3.left;
                break;
            case 3:
                action = Vector3.forward;
                break;
            case 4:
                action = Vector3.back;
                break;

                
        }

        _cc.Move(action * movementSpeed);
        float currDist = Mathf.Pow(Vector3.Distance(transform.position, target.transform.position), 2);
        float prevDist = Mathf.Pow(Vector3.Distance(prevPos, target.transform.position), 2);

        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit hit, 1000f, targetMask))
        {
            if(hit.transform.tag == "Target")
            {
                if(currDist < prevDist)
                {
                    AddReward(0.1f);
                }
                else
                {
                    AddReward(-0.1f);
                }
            }
            else
            {
                bool density = densityModel.GetVisited(currentState);
                float intrinsicReward = !density ? 0.2f : -0.02f;
                AddReward(intrinsicReward);
            }
        }

        
        

        
        densityModel.AddState(currentState);


        

        


        
        agentStatsUI.SetBestReward(GetCumulativeReward());
        agentStatsUI.SetText(disAction, GetCumulativeReward());

        prevPos = transform.position;

        UpdateHeatMap();
    }

    private void CreateHeatMap()
    {
        GameObject map = new GameObject();
        heatMapTiles = new GameObject[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(min.x + x * cellSize, 0, -20 + min.z + z * cellSize);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tile.transform.parent = map.transform;
                tile.transform.position = position;
                tile.transform.rotation = Quaternion.Euler(90, 0, 0); // Rotate to lie flat on the ground
                tile.transform.localScale = new Vector3(cellSize, cellSize, 1);
                heatMapTiles[x, z] = tile;
            }
        }
    }

    private void UpdateHeatMap()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(min.x + x * cellSize, 0, min.z + z * cellSize);
                bool density = densityModel.GetVisited(position);
                Color color = Color.Lerp(Color.red, Color.green, density ? 1 : 0); // Adjust the divisor for scaling
                heatMapTiles[x, z].GetComponent<Renderer>().material.color = color;
            }
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var discreteOut = actionsOut.DiscreteActions;
        discreteOut[0] = Input.GetAxis("Horizontal") > 0 ? 1 : Input.GetAxis("Horizontal") < 0 ? 2 : Input.GetAxis("Vertical") > 0 ? 3 : Input.GetAxis("Vertical") < 0 ? 4 : 0;
        //var contOut = actionsOut.ContinuousActions;
        //contOut[0] = Input.GetAxis("Horizontal");
        //contOut[1] = Input.GetAxis("Vertical");
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.name == "Wall")
        {
            AddReward(-0.1f);
        }
        if(hit.gameObject.name == "Target")
        {
            AddReward(1000);
            agentStatsUI.SetBestReward(GetCumulativeReward());
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.transform.position);
        sensor.AddObservation(transform.position - target.transform.position);
        sensor.AddObservation(min);
        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit hit, 1000f, targetMask))
        {
            if (hit.transform.tag == "Target")
            {
                sensor.AddObservation(true);
            }
            else
            {
                sensor.AddObservation(false);
            }
        }

        sensor.AddObservation(densityModel.FlattenHeatMap());

        
    }
}



