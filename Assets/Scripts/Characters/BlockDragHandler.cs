using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class BlockDragHandler : BlockCharacter
{
    // Start is called before the first frame update
    [SerializeField]
    public LayerMask gridLayer;
    public List<ChildBlock> ChildBlocks = new List<ChildBlock>();
    public List<ChildBlock> ChildWithTargets = new List<ChildBlock>();
    public Transform Mesh;
    public Vector3 MeshGoalPos;
    public Vector3 MeshGoalSize;
    public BlockGridInfo LastBlockGridInfo;
    // public Transform SpawnPosition;
    // public int Multiplier;
    // public GameObject ParticleBlock;
    // public List<Transform> IntroPath = new List<Transform>();
    // public float IntroPathSeconds = 1;
    public BlockGoal BlockGoal;
    Rigidbody rb;
    public BoxCollider BoxCLD;
    void Start()
    {
        BoxCLD = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        BlockGoal.ListAllBlocks.Add(this);
    }
    void Update()
    {
        if (!BlockManager.Instance.IsDragging)
        {
        if (MainGridCell.GetGrid() != null && MainGridCell.GetGrid().Grid != null )
        {
            var grid = MainGridCell.GetGrid();
            if (grid != null)
            {
                Vector3 snappedPos = new Vector3(grid.Grid.GetGridX(), transform.position.y, grid.Grid.GetGridZ());
                if (transform.position != snappedPos)
                {
                    SnapPosition(grid);
                }
            }
        }
        }  
    }
    public override void AttachOnGrid(BlockGridInfo gridCell)
    {
        try
        {
            MainGridCell.SetGrid(gridCell.GetGrid());
            MainGridCell.SetDistanceOfGrid(gridCell.GetDistanceOfGrid());

            gridCell.GetGrid().AttachBlockCharacter(this);
            SnapPosition(gridCell.GetGrid());
            ChildrenAttachOnGrid();
            IsChildrenOnTargetGrid();
        }
        catch (System.Exception)
        {
        }


    }
    
    public void ChildrenAttachOnGrid()
    {
        foreach (var item in ChildBlocks)
        {
            if (item != null)
            {
                if (item.MainGridCell.GetGrid() != null && item.MainGridCell.GetGrid() != null)
                {
                    item.MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
                }
                (GridCell _cell, float _distance) nearestGridCell = BlockManager.Instance.CheckNearestGrid(new GridHelper(item.transform.position.x,
                                                                                                item.transform.position.z),
                                                                                                item.MainGridCell.GetGrid());

                item.AttachOnGrid(new BlockGridInfo
                (
                    nearestGridCell._cell,
                    nearestGridCell._distance
                ));
            }
        }
    }
    public void IsChildrenOnTargetGrid()
    {
        foreach (var child in ChildWithTargets)
        {
            if (child.TargetGridCellSecond != null)
            {
                if (child.MainGridCell.GetGrid() == child.TargetGridCell)
                {
                    break;
                }
                else if (child.MainGridCell.GetGrid() == child.TargetGridCellSecond)
                {
                    break;
                }
                return;
            }
            else
            {
                if (child.MainGridCell.GetGrid() != child.TargetGridCell)
                {
                    return;
                }
            }
            
        }
        InitializeMultiply();
    }
    public void InitializeMultiply()
    {
        GetComponent<BoxCollider>().enabled = false;
        MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
        foreach (var item in ChildBlocks)
        {

            if (item != null)
            {
                if (item.MainGridCell.GetGrid() != null && item.MainGridCell.GetGrid() != null)
                {
                    item.MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
                }


            }
        }
        foreach (var child in ChildBlocks)
        {
            child.gameObject.SetActive(false);
        }
        float delay = 0.25f; 
        int total = ChildBlocks.Count * (int)BlockGoal.Multiplier; 
        float spawnDuration = total * delay;
        Mesh.transform.DOLocalMove(MeshGoalPos, spawnDuration).SetEase(Ease.Linear);
        Mesh.transform.DOScale(MeshGoalSize, spawnDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            Mesh.GetComponent<MeshRenderer>().enabled = false;
            Mesh.localScale = Vector3.one;
        });
        BlockGoal.LastBlockDragHandler = this;
        BlockGoal.BlockVFX.SetActive(true);
        StartCoroutine(spawnChildren(this));
    }
    IEnumerator spawnChildren(BlockDragHandler oldBlockDragHandler)
    {
        int total = ChildBlocks.Count * (int)BlockGoal.Multiplier;
        GameObject idle = ChildBlocks[0].gameObject;
        for (int i = 0; i < total; i++)
        {
            GameObject newChild = Instantiate(idle, BlockGoal.SpawnPosition.position, Quaternion.identity);
            newChild.SetActive(true);
            newChild.transform.position = BlockGoal.SpawnPosition.position;
            newChild.transform.localScale = new Vector3(0.519999981f, 0.519999981f, 0.519999981f);
            newChild.GetComponent<ChildBlock>().Parent = this;
            BattleController.Instance.FindEnemy(newChild.GetComponent<ChildBlock>());
            yield return new WaitForSeconds(0.25f);
        }
        if (oldBlockDragHandler == BlockGoal.LastBlockDragHandler)
        {
            BlockGoal.BlockVFX.SetActive(false);
        }
        BlockGoal.RemoveBlock(this);
    }

    public void SnapPosition(GridCell gridCell)
    {
        // transform.position = new Vector3(gridCell.Grid.GetGridX(),
        //                                 transform.position.y,
        //                                 gridCell.Grid.GetGridZ());
        Vector3 target = new Vector3(
        gridCell.Grid.GetGridX(),
        transform.position.y,
        gridCell.Grid.GetGridZ());

        transform.DOMove(target, 0.1f).SetEase(Ease.OutQuad);
    }
    [ContextMenu("GetPos")]
    public void GetPos() => Debug.Log($"Grid X {transform.position.x} , Grid Z {transform.position.z}");
}
