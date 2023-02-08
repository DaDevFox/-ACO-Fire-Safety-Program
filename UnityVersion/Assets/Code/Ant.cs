using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Ant
{
    public Mesh mesh = ((GameObject)Resources.Load("Ant.prefab")).GetComponent<MeshFilter>().mesh;
    public Material material = ((GameObject)Resources.Load("Ant.prefab")).GetComponent<MeshRenderer>().material;

    public Vector3 position;
    /// <summary>
    /// does not include current position
    /// </summary>
    public List<Vector3> visited;
    public Vector3[] currentNeighbors;

    public static float guidingPheremoneCoefficient = 1f;
    public static float standardPheremoneCoefficient = 0.5f;

    public Vector3[] path;

    public void Iterate()
    {
        if(path != null)
        {
            // set new pos
            int newNeighbor = ChooseNewNeighbor();
            if (newNeighbor != -1)
            {
                if (visited.Count > 0)
                {
                    AntSystem.DepositStandardPheremone(position, 1f / (position - visited[visited.Count - 1]).sqrMagnitude);
                }

                visited.Add(position);
                position = currentNeighbors[newNeighbor];
                currentNeighbors = AntSystem.GetNeighbors(position);
            }

            // check for end condition
            if(position == AntSystem.main.end)
            {
                Debug.Log("This is the end");
            }
        }
    }

    public void Draw()
    {
        Graphics.DrawMesh(mesh, position, Quaternion.identity, material, 0);
    }

    public int ChooseNewNeighbor()
    {
        float totalPheremone = currentNeighbors.Sum((neighbor) => AntSystem.GuidingPheremone(neighbor) + AntSystem.StandardPheremone(neighbor));
        float[] probabilities = new float[currentNeighbors.Length];

        foreach (Vector3 neighbor in currentNeighbors)
        {
            float pheremone = AntSystem.GuidingPheremone(neighbor) + AntSystem.StandardPheremone(neighbor);
            float probability = pheremone / totalPheremone;
        }

        float random = UnityEngine.Random.Range(0f, 1f);
        float sum = 0f;
        int chosen = -1;
        for (int i = 0; i < probabilities.Length; i++)
        {
            if (random > sum && random < probabilities[i])
            {
                chosen = i;
                break;
            }
            sum += probabilities[i];
        }

        return chosen;
    }
}

