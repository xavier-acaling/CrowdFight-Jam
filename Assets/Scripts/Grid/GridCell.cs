using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public BlockCharacter OccupiedCharacter;
    public GridHelper Grid;
    void Awake()
    {
        init();
    }
    void Start()
    {
        if (!BlockManager.Instance.AllGridCells.Contains(this))
        {
            BlockManager.Instance.AllGridCells.Add(this);
        }
    }
    void init()
    {
        Grid = new GridHelper(transform.position.x, transform.position.z);
    }
    public void AttachBlockCharacter(BlockCharacter blockCharacter)
    {
        Debug.Log($"AttachBlockCharacter ");
        if (OccupiedCharacter == null)
        {
            OccupiedCharacter = blockCharacter;
        }
    }
    public void UnattachCurrentBlockCharacter()
    {
        if (OccupiedCharacter != null)
        {
            OccupiedCharacter = null;
            Debug.Log($"OccupiedCharacter null {OccupiedCharacter == null}");
        }
    }
}
