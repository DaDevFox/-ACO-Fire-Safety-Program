using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR

//[ExecuteInEditMode]
//#endif
public class PathingGraph : MonoBehaviour
{
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

    public bool Node(Vector3 position) => nodes.ContainsKey(position) ? nodes[position] : false;

    private void Start()
    {
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
        if (pathfinderStart && pathfinderEnd)
        {
            pathfinder.start = pathfinderStart.transform.position;
            pathfinder.end = pathfinderEnd.transform.position;

            //if (Input.GetKeyDown(KeyCode.P))
                pathfinder.Path();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = Color.blue;
        foreach (Vector3 node in nodes.Keys)
            if (nodes[node])
                Gizmos.DrawCube(node, Vector3.one * 0.1f);

        if(pathfinder.result != null && pathfinder.result.Length > 0)
        {
            for(int i = 0; i < pathfinder.result.Length; i++)
            {
                Color color = Color.green;
                color.a = 1f;
                Gizmos.color = color;
                Gizmos.DrawCube(pathfinder.result[i] + resultHoverOffset, Vector3.one * 0.2f);
            }
        }
    }
}
