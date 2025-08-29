using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BlockGridInfo
{
    [SerializeField]
    public GridCell grid;
    [SerializeField]
    private float distanceOfGrid;
    public BlockGridInfo(GridCell gridCell, float distance)
    {
        grid = gridCell;
        distanceOfGrid = distance;
    }
    public GridCell GetGrid() => grid;
    public float GetDistanceOfGrid() => distanceOfGrid;
    public void SetGrid(GridCell _grid) => grid = _grid;
    public void SetDistanceOfGrid(float _distance) => distanceOfGrid = _distance;
}
