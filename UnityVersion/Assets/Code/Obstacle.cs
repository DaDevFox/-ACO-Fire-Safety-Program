using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector3 location;
    public Vector3 size;

    internal bool initialized = false;

    private void Start()
    {
        Initialize();
    }

    internal void Initialize()
    {
        if(initialized) return;

        if (GetComponent<BoxCollider>())
        {
            size = new Vector3(GetComponent<BoxCollider>().size.x * transform.localScale.x, GetComponent<BoxCollider>().size.y * transform.localScale.y, GetComponent<BoxCollider>().size.z * transform.localScale.z);
        }

        if (location == Vector3.zero) 
            location = transform.position - size/2f;
        
        initialized = true;
    }

    public bool CheckInside(Vector3 pos)
    {
        return ((pos.x >= location.x && pos.z >= location.z) && (pos.x <= location.x + size.x && pos.z <= location.z + size.z));
    }
}
