using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AntSystem : Pathfinder
{
    /// <summary>
    /// laid after target is found; returning to start
    /// </summary>
    //public Dictionary<Vector3, float> guidingPheremones = new Dictionary<Vector3, float>();
    //public Dictionary<Vector3, float> standardPheremones = new Dictionary<Vector3, float>();

    public float[,] guidingPheremones;
    public float[,] standardPheremones;

    private int offsetX;
    private int offsetZ;

    public static AntSystem main;

    [HideInInspector]
    public Ant[] ants;

    public int agentCount = 10;
    public float startPheremone = 0.1f;
    public int iterations = 1000;

    public int displayingAnt = 0;
    public bool displayStandardPheremone = false;
    public bool displayGuidingPheremone = false;


    private Vector3 antHover = new Vector3(0f, 1f, 0f);

    public static void DepositStandardPheremone(Vector3 location, float amount)
    {
        main.standardPheremones[(int)location.x + main.offsetX, (int)location.z + main.offsetZ] += amount;
        //if (!main.standardPheremones.ContainsKey(location))
        //    main.standardPheremones.Add(location, 0f);
        //main.standardPheremones[new Vector3((int)location.x, 0f, (int)location.z)] += amount;
    }

    public static void DepositGuidingPheremone(Vector3 location, float amount)
    {
        main.guidingPheremones[(int)location.x + main.offsetX, (int)location.z + main.offsetZ] += amount;
        //if (!main.guidingPheremones.ContainsKey(location))
        //    main.guidingPheremones.Add(location, 0f);
        //main.guidingPheremones[new Vector3((int)location.x, 0f, (int)location.z)] += amount;
    }

    public static float GuidingPheremone(Vector3 location) => 
        //main.guidingPheremones.ContainsKey(location) ? main.guidingPheremones[new Vector3((int)location.x, 0f, (int)location.z)] : 0f;
    main.guidingPheremones[(int)location.x + main.offsetX, (int)location.z + main.offsetZ];
    public static float StandardPheremone(Vector3 location) => 
        //main.standardPheremones.ContainsKey(location) ? main.standardPheremones[new Vector3((int)location.x, 0f, (int)location.z)] : 0f;
    main.standardPheremones[(int)location.x + main.offsetX, (int)location.z + main.offsetZ];

    public static Vector3[] GetNeighbors(Vector3 location, out int count) => main.graph.GetNeighbors(location, out count);

    public override void Path()
    {
        Initialize();

        Vector3[] best = null;
        for (int idx = 0; idx < ants.Length; idx++)
        {
            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    ants[idx].Iterate();
                }catch(Exception ex)
                {
                    Debug.Log($"Ant Exception: \n{ex}");
                }

                if (ants[idx].path != null)
                    if(best == null || ants[idx].path.Length < best.Length)
                        best = ants[idx].path;
            }
        }

        //if (best != null)
        //    Debug.Log($"Completed: path of {best.Length} nodes found");
        //else
        //    Debug.Log($"No path found in {iterations} iterations");
        
        result = best;
    }

    public void OnDrawGizmosSelected()
    {
        if(displayingAnt < agentCount && displayingAnt >= 0)
        {
            Gizmos.color = ants[displayingAnt].path == null ? Color.red : Color.green;
            for(int i = 0; i < ants[displayingAnt].visited.Count; i++)
            {
                Gizmos.DrawSphere(ants[displayingAnt].visited[i], 0.2f);
            }
        }

        
        float standardHeight = 1f;
        float guidingHeight = 2f;

        float cubeSize = 0.2f;

        for(float x = -graph.dimensions.x/2f; x < graph.dimensions.x/2f; x++)
        {
            for(float z = -graph.dimensions.y/2f; z < graph.dimensions.y/2f; z++)
            {
                if (displayStandardPheremone)
                {
                    if (StandardPheremone(new Vector3((int)x, 0f, (int)z)) > 0f)
                    {
                        Gizmos.color = Color.blue * StandardPheremone(new Vector3((int)x, 0f, (int)z));
                        Gizmos.DrawCube(new Vector3((int)x, standardHeight, (int)z), Vector3.one * cubeSize);
                    }
                }
                if (displayGuidingPheremone)
                {
                    if (GuidingPheremone(new Vector3((int)x, 0f, (int)z)) > 0f)
                    {
                        Gizmos.color = new Color(1f, 0f, 1f) * (GuidingPheremone(new Vector3((int)x, 0f, (int)z)) / Ant.guidingPheremonePerPath);
                        Gizmos.DrawCube(new Vector3((int)x, guidingHeight, (int)z), Vector3.one * cubeSize);
                    }
                }
            }
        }
    }


    //public IEnumerator DebugAntCoroutine()
    //{
    //    int hardIterations = 600;
    //    Gizmos.color = Color.grey;

    //    int iterationInterval = 60;
    //    for (int i = 0; i < hardIterations; i++)
    //    {
    //        foreach (Ant ant in ants) 
    //        {
    //            if (i / iterationInterval == 0)
    //                ant.Iterate();
    //            ant.Draw();
    //        }

    //        yield return new WaitForEndOfFrame();
    //    }

    //}

    

    //private void Start()
    //{
    //    Initialize();
    //}

    private void Initialize()
    {
        main = this;

        ants = new Ant[agentCount];
        guidingPheremones = new float[(int)(graph.dimensions.x), (int)(graph.dimensions.y)];
        standardPheremones = new float[(int)(graph.dimensions.x), (int)(graph.dimensions.y)];

        //guidingPheremones.Clear();
        //standardPheremones.Clear();

        Vector3[] neighbors = graph.GetNeighbors(start, out int count);
        for (int i = 0; i < agentCount; i++)
        {
            Ant ant = new Ant()
            {
                position = start
            };
            ants[i] = ant;

            for(int j = 0; j < count; j++)
            {
                ant.currentNeighbors[j] = neighbors[j];
            }
        }

        offsetX = (int)(graph.dimensions.x * 0.5f);
        offsetZ = (int)(graph.dimensions.y * 0.5f);
        for (float x = -graph.dimensions.x / 2f; x < graph.dimensions.x/2f; x++)
        {
            for(float z = -graph.dimensions.y/2f; z < graph.dimensions.y/2f; z++)
            {
                //DepositStandardPheremone(new Vector3((int)x, 0f, (int)z), 1f);
                standardPheremones[(int)x + offsetX, (int)z + offsetZ] += 1f;
            }
        }
    }
}
