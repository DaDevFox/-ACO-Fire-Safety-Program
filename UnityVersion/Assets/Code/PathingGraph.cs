using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Profiling;
//#if UNITY_EDITOR

//[ExecuteInEditMode]
//#endif
public class PathingGraph : MonoBehaviour
{
    public enum Direction
    {
        North,
        East,
        South,
        West,
        Northeast,
        Southeast,
        Southwest,
        Northwest
    }

    [Header("Settings")]
    public Pathfinder pathfinder;
    public Vector2 dimensions;
    public Vector3 resultHoverOffset;

    [Header("Scene")]
    public GameObject pathfinderStart;
    public GameObject pathfinderEnd;
    public GameObject obstaclesContainer;
    public List<Obstacle> obstacles = new List<Obstacle>();


    private Dictionary<Vector3, bool> nodes = new Dictionary<Vector3, bool>();

    private Dictionary<Direction, Vector3> directionVectors = new Dictionary<Direction, Vector3>()
    {
        { Direction.North, new Vector3(0, 0, -1)},
        { Direction.East, new Vector3(1, 0, 0)},
        { Direction.South, new Vector3(0, 0, 1)},
        { Direction.West, new Vector3(-1, 0, 0)},
        { Direction.Northeast, new Vector3(1, 0, -1)},
        { Direction.Southeast, new Vector3(1, 0, 1)},
        { Direction.Southwest, new Vector3(-1, 0, 1)},
        { Direction.Northwest, new Vector3(-1, 0, -1)}
    };

    private Vector3[] neighbors = new Vector3[9];

    public Vector3[] GetNeighbors(Vector3 node, out int count)
    {
        node = new Vector3((int)node.x, 0f, (int)node.z);

        count = 0;
        foreach(Vector3 delta in directionVectors.Values)
        {
            Vector3 test = node + delta;
            if (Node(test))
            {
                neighbors[count] = test;
                count++;
            }
        }
        return neighbors;
    }

    public bool Node(Vector3 position) => nodes.ContainsKey(position) ? nodes[position] : false;

    private void Start()
    {
        //Profiler.enableAllocationCallstacks = true;

        BakeObstacles();
        Remap();

        pathfinder.graph = this;
    }

    private void BakeObstacles()
    {
        obstacles.Clear();
        for (int i = 0; i < obstaclesContainer.transform.childCount; i++)
        {
            Obstacle obstacle = obstaclesContainer.transform.GetChild(i).GetComponent<Obstacle>();
            if (obstacle)
            {
                obstacles.Add(obstacle);
                if (!obstacle.initialized)
                    obstacle.Initialize();
            }
        }
    }

    private void Remap()
    {
        for (float x = -dimensions.x / 2f; x < dimensions.x / 2f; x++)
        {
            for (float z = -dimensions.y / 2f; z < dimensions.y / 2f; z++)
            {
                if (!Blocked(new Vector3(x, 0f, z)))
                
                    nodes.Add(new Vector3(x, 0f, z), true);
                else
                    nodes.Add(new Vector3(x, 0f, z), false);
            }
        }
    }

    private bool Blocked(Vector3 pos)
    {
        foreach (Obstacle obstacle in obstacles)
            if (obstacle.CheckInside(pos))
                return true;
        return false;
    }

    public void Update()
    {
        //if (pathfinderStart && pathfinderEnd)
        //{
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                pathfinder.start = pathfinderStart.transform.position;
                pathfinder.end = pathfinderEnd.transform.position;


                Stopwatch timer = new Stopwatch();
                timer.Start();
                pathfinder.Path();
                timer.Stop();

                UnityEngine.Debug.Log($"{pathfinder.GetType().Name} completed pathing operation in {timer.ElapsedMilliseconds} ms");
            }
        //}
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.blue;
        foreach (Vector3 node in nodes.Keys)
            if (nodes[node])
                Gizmos.DrawCube(node, Vector3.one * 0.1f);

        if(pathfinder.result != null && pathfinder.result.Length > 0)
        {
            Color color = Color.green;
            color.a = 1f;
            Gizmos.color = color;

            for (int i = 0; i < pathfinder.result.Length; i++)
            {
                Gizmos.DrawCube(pathfinder.result[i] + resultHoverOffset, Vector3.one * 0.2f);
            }
        }

        if (pathfinder is AntSystem antSystem)
        {
            Gizmos.color = Color.black;
            foreach(Ant ant in antSystem.ants)
            {
                Gizmos.DrawCube(ant.position, Vector3.one * 0.3f);
            }
        }

    }
}
