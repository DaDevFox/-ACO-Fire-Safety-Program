using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHusse.Pathfinding;
using UnityEngine;
using DeenGames.Utils.AStarPathFinder;

public class ExternalAStarPathfinder : Pathfinder
{
    public class Context
    {
        public ExternalAStarPathfinder pathfinder;
        public Node node;
        public bool walkable => node.walkable;
        public Vector3 uintPosition => new Vector3((int)graphPosition.x + (int)(pathfinder.graph.dimensions.x / 2f), 0f, (int)graphPosition.z + (int)(pathfinder.graph.dimensions.y / 2f));
        public Vector3 graphPosition { get; private set; } = Vector3.zero;

        public Context(ExternalAStarPathfinder pathfinder, int x, int z)
        {
            this.pathfinder = pathfinder;
            graphPosition = new Vector3(x, 0f, z);
            node = new Node(this, (int)uintPosition.x, (int)uintPosition.z);
        }

        public Context(ExternalAStarPathfinder pathfinder, Vector3 pos)
        {
            this.pathfinder = pathfinder;
            graphPosition = pos;
            node = new Node(this, (int)uintPosition.x, (int)uintPosition.z);
        }
    }

    public class Node : IPathNode<Context>
    {
        public int X => x;
        public int Y => y;

        public int x = 0;
        public int y = 0;

        public bool walkable = false;
        public Context context { get; private set; }

        public Node(Context context)
        {
            this.context = context;
        }

        public Node(Context context, int x, int y)
        {
            this.context = context;
            this.x = x;
            this.y = y;
        }

        public Context GetContext() => this.context;

        public bool IsWalkable(Context inContext)
        {
            return inContext.walkable;
        }
    }

    private Node[,] grid;
    private bool initialized = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize(bool force = false)
    {
        if (initialized && !force)
            return;

        // FLAG: Accurate for non integer dimensions in x or y?
        grid = new Node[(int)graph.dimensions.x, (int)graph.dimensions.y];

        for(float x = -graph.dimensions.x/2f; x < graph.dimensions.x/2f; x++)
        {
            for(float z = -graph.dimensions.y/2f; z < graph.dimensions.y/2f; z++)
            {
                int xInt = (int)x + (int)(graph.dimensions.x / 2f);
                int zInt = (int)z + (int)(graph.dimensions.y / 2f);
                Context context = new Context(this, new Vector3(x, 0f, z));
                grid[xInt, zInt] = context.node;

                if (graph.Node(new Vector3(x, 0f, z)))
                {
                    context.node.walkable = true;
                    Debug.Log($"{xInt}, {zInt}");
                }
            }
        }

        initialized = true;
    }

    public override void Path()
    {
        Initialize();

        SpatialAStar<Node, Context> pather = new SpatialAStar<Node, Context>(grid);

        LinkedList<Node> result = pather.Search(
            grid[(int)start.x + (int)(graph.dimensions.x / 2f), (int)start.z + (int)(graph.dimensions.y / 2f)], 
            grid[(int)end.x + (int)(graph.dimensions.x / 2f), (int)end.z + (int)(graph.dimensions.y / 2f)], grid[(int)start.x + (int)(graph.dimensions.x / 2f), (int)start.z + (int)(graph.dimensions.y / 2f)].context);
        List<Vector3> resultVectors = new List<Vector3>();
        foreach(Node node in result)
        {
            resultVectors.Add(node.GetContext().graphPosition);
        }
        this.result = (Vector3[])resultVectors.ToArray();
    }
}

