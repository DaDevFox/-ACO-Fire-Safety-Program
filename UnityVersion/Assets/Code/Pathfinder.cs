using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class Pathfinder : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;

    public PathingGraph graph;

    public Vector3[] result;
    
    // asdf

    public abstract void Path();
}

