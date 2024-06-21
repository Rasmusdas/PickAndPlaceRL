using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Sentis.Layers;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public int agentAmount;

    public GameObject playField;
    public PrefabManager prefabManager => PrefabManager.instance;

    void Start()
    {
        var pf = Instantiate(playField).GetComponent<PlayField>();

        pf.LoadRandomLevel();

        var agent = Instantiate(prefabManager[TileType.Agent]).GetComponentInChildren<PickupAgent>();

        agent.pF = pf;

        pf.SetAgent(agent);
    }

}
