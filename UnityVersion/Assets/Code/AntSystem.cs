using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSystem : Pathfinder
{
    /// <summary>
    /// laid after target is found; returning to start
    /// </summary>
    public Dictionary<Vector3, float> guidingPheremones = new Dictionary<Vector3, float>();
    public Dictionary<Vector3, float> standardPheremones = new Dictionary<Vector3, float>();

    public static AntSystem main { get; private set; }

    [HideInInspector]
    public Ant[] ants;

    public int agentCount = 100;

    private Vector3 antHover = new Vector3(0f, 1f, 0f);

    public static void DepositStandardPheremone(Vector3 location, float amount)
    {
        if (!main.standardPheremones.ContainsKey(location))
            main.standardPheremones.Add(location, 0f);
        main.standardPheremones[location] += amount;
    }

    public static void DepositGuidingPheremone(Vector3 location, float amount)
    {
        if (!main.guidingPheremones.ContainsKey(location))
            main.guidingPheremones.Add(location, 0f);
        main.guidingPheremones[location] += amount;
    }

    public static float GuidingPheremone(Vector3 location) => main.guidingPheremones.ContainsKey(location) ? main.guidingPheremones[location] : 0f;
    public static float StandardPheremone(Vector3 location) => main.standardPheremones.ContainsKey(location) ? main.standardPheremones[location] : 0f;

    public static Vector3[] GetNeighbors(Vector3 location) => main.graph.GetNeighbors(location);

    public override void Path()
    {
        Initialize();



        StartCoroutine("DebugAntCoroutine");
    }

    public IEnumerator DebugAntCoroutine()
    {
        int hardIterations = 600;
        Gizmos.color = Color.grey;

        int iterationInterval = 60;
        for (int i = 0; i < hardIterations; i++)
        {
            foreach (Ant ant in ants) 
            {
                if (i / iterationInterval == 0)
                    ant.Iterate();
                ant.Draw();
            }

            yield return new WaitForEndOfFrame();
        }

    }

    

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        main = this;

        ants = new Ant[agentCount];
        for(int i = 0; i < agentCount; i++)
        {
            ants[i] = new Ant();
            ants[i].position = start;
        }
    }
}
