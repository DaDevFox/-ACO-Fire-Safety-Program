
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;

    public class OldAnt
    {
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

        public Vector3[] path;

        private float[] probabilities = new float[9];


        public void Iterate()
        {
            if (path == null)
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
                    Vector3[] newNeighbors = AntSystem.GetNeighbors(position, out int count);

                    currentNeighborsLength = count;
                    for (int i = 0; i < currentNeighborsLength; i++)
                    {
                        currentNeighbors[i] = newNeighbors[i];
                    }
                }

                // check for end condition
                if (position == new Vector3((int)AntSystem.main.end.x, 0f, (int)AntSystem.main.end.z))
                {
                    path = new Vector3[visited.Count];
                    path[0] = visited[0];
                    for (int i = 1; i < visited.Count; i++)
                    {
                        path[i] = visited[i];
                        AntSystem.DepositGuidingPheremone(visited[i], 1f / (visited[i] - visited[i - 1]).sqrMagnitude);
                    }
                }
            }
        }

        //public void Draw()
        //{
        //    Graphics.DrawMesh(mesh, position, Quaternion.identity, material, 0);
        //}
        public int ChooseNewNeighbor()
        {
            int chosen = -1;

            float totalPheremone = 0f;
            for (int i = 0; i < currentNeighborsLength; i++)
            {
                if (!visited.Contains(currentNeighbors[i]))
                    totalPheremone += AntSystem.GuidingPheremone(currentNeighbors[i]) * guidingPheremoneCoefficient + AntSystem.StandardPheremone(currentNeighbors[i]) * standardPheremoneCoefficient;
            }

            for (int i = 0; i < currentNeighborsLength; i++)
            {
                if (!visited.Contains(currentNeighbors[i]))
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

