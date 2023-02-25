using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Ant
{
    private class VectorComparer : IComparer<Vector3>
    {
        private AntSystem system;
        public VectorComparer(AntSystem system)
        {
            this.system = system;
        }

        public int Compare(Vector3 x, Vector3 y) => Ant.CompareVectors(x, y);
    }

    //public static Mesh mesh = ((GameObject)Resources.Load("Ant")).GetComponent<MeshFilter>().sharedMesh;
    //public static Material material = ((GameObject)Resources.Load("Ant")).GetComponent<MeshRenderer>().sharedMaterial;

    public Vector3 position;
    /// <summary>
    /// does not include current position
    /// </summary>
    public List<Vector3> visited = new List<Vector3>();
    private int currentNeighborsLength = 9;
    public Vector3[] currentNeighbors = new Vector3[9];

    public static float guidingPheremoneCoefficient = 2000000f;
    public static float standardPheremoneCoefficient = 0.1f;

    public static float baseStandardPheremone = 0.1f;
    public static float guidingPheremonePerPath = 1000f;

    public Vector3[] path;

    private float[] probabilities = new float[9];

    private VectorComparer comparer;

    public Ant()
    {
        comparer = new VectorComparer(AntSystem.main);
    }

    private static int CompareVectors(Vector3 a, Vector3 b)
    {
        float dimensionsX = AntSystem.main.graph.dimensions.x * 0.5f;
        float dimensionsY = AntSystem.main.graph.dimensions.y * 0.5f;
        a.x += dimensionsX;
        a.z += dimensionsY;
        b.x += dimensionsX;
        b.z += dimensionsY;

        return ((int)a.x + (int)(2f * dimensionsX * (int)a.z)) 
            - 
            ((int)b.x + (int)(2f * dimensionsX * (int)b.z));
    }

    public void Iterate()
    {
        if(path == null)
        {
            // set new pos
            int newNeighbor = ChooseNewNeighbor();
            if (newNeighbor != -1)
            {
                if (visited.Count > 0)
                {
                    AntSystem.DepositStandardPheremone(position, baseStandardPheremone);
                }

                visited.InsertIntoSortedList(position, CompareVectors);
                position = currentNeighbors[newNeighbor];
                Vector3[] newNeighbors = AntSystem.main.graph.GetNeighbors(position, out int count);

                currentNeighborsLength = count;
                for(int i = 0; i < currentNeighborsLength; i++)
                {
                    currentNeighbors[i] = newNeighbors[i];
                }
            }

            // check for end condition
            if (position == new Vector3((int)AntSystem.main.end.x, 0f, (int)AntSystem.main.end.z))
            {
                path = new Vector3[visited.Count];
                path[0] = visited[0];
                float pheremoneAmount = guidingPheremonePerPath;
                
                for(int i = 1; i < visited.Count; i++) 
                {
                    path[i] = visited[i];
                    AntSystem.DepositGuidingPheremone(visited[i], pheremoneAmount);
                }
            }
        }
    }

    private bool Visited(Vector3 location)
    {
        for(int i = 0; i < visited.Count; i++)
        {
            if ((int)location.x == (int)visited[i].x && (int)location.z == (int)visited[i].z)
                return true;
        }
        return false;
    }

    private int[] searchResults = new int[9];

    public int ChooseNewNeighbor()
    {
        int chosen = -1;

        float totalPheremone = 0f;
        for(int i = 0; i < currentNeighborsLength; i++)
        {
            searchResults[i] = visited.BinarySearch(currentNeighbors[i], comparer);
            if (searchResults[i] < 0)
                totalPheremone += AntSystem.GuidingPheremone(currentNeighbors[i]) * guidingPheremoneCoefficient + AntSystem.StandardPheremone(currentNeighbors[i]) * standardPheremoneCoefficient;
        }

        for(int i = 0; i < currentNeighborsLength; i++)
        {
            if (searchResults[i] < 0)
            {
                float pheremone = AntSystem.GuidingPheremone(currentNeighbors[i]) * guidingPheremoneCoefficient + AntSystem.StandardPheremone(currentNeighbors[i]) * standardPheremoneCoefficient;

                if (currentNeighbors[i] == new Vector3((int)AntSystem.main.end.x, 0f, (int)AntSystem.main.end.z))
                {
                    chosen = i;
                    break;
                }

                float probability = pheremone / totalPheremone;
                probabilities[i] = probability;
            }
            else
                probabilities[i] = 0f;
        }

        float random = UnityEngine.Random.Range(0f, 1f);
        float sum = 0f;
        for (int i = 0; i < currentNeighborsLength; i++)
        {
            if (random > sum && random < sum + probabilities[i])
            {
                chosen = i;
                break;
            }
            sum += probabilities[i];
        }

        return chosen;
    }
}

