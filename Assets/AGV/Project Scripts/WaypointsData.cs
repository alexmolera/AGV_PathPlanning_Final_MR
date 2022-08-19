using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaypointsData
{
    public int road;
    public float x;
    public float y;

    //Constructor empty
    public WaypointsData()
    {
        road = 0;
        x = 0;
        y = 0;
    }
    //Constructor
    public WaypointsData(Vector3 _pos, int _road)
    {
        road = _road;
        x = _pos.x;
        y = _pos.z;
    }
}
