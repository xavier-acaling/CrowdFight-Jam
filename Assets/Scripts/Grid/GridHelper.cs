using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridHelper
{
    [SerializeField]
    private float gridX;
    [SerializeField]
    private float gridZ;
    public GridHelper(float x, float z)
    {
        gridZ = z;
        gridX = x;
    }
    public float GetGridZ() => gridZ; 
    public float GetGridX() => gridX; 
}
