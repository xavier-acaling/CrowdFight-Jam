using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildBlock : BlockCharacter
{
    public BlockDragHandler Parent;
    public GridCell TargetGridCell;
    public GridCell TargetGridCellSecond;
    public Enemy TargetEnemy;
    public Rigidbody RB;
    public Animator Anim;
    void Start()
    {
        Anim = GetComponent<Animator>();
        if (transform.parent != null)
        {
            if (transform.parent.GetComponent<BlockDragHandler>() != null)
            {
                Parent = transform.parent.GetComponent<BlockDragHandler>();

            }

        }
    }
    public override void AttachOnGrid(BlockGridInfo gridCell)
    {
        MainGridCell.SetGrid(gridCell.GetGrid());
        MainGridCell.SetDistanceOfGrid(gridCell.GetDistanceOfGrid());

        gridCell.GetGrid().AttachBlockCharacter(this);
    }
    void Update()
    {
        // if (MainGridCell.GetGrid() != null)
        // {
        //     if (MainGridCell.GetGrid().OccupiedCharacter != this)
        //     {
        //         MainGridCell.GetGrid().AttachBlockCharacter(this);
        //     }
        // }  
    }
}
