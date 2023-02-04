using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSafetyACO.Simulation
{
    public class Point
    {
        public float x;
        public float y;
        // UNUSED?
        public float z;

        public Point()
        {
            x = 0; y = 0; z = 0;
        }

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0f;
        }

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Node
    {
        public Point location;
        public Node[] connections;

        public Node(Point location)
        {
            this.location = location;
        }
    }

    public class PathingGraph
    {
        




    }
}
