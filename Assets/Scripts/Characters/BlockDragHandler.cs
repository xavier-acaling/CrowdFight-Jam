using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class BlockDragHandler : BlockCharacter
{
    // Start is called before the first frame update
    [Header("Feature")]
    public HVFeature HorizontalVerticalFeature;
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
    public bool Multiply;
    void Start()
    {
        BoxCLD = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        if (!BlockManager.Instance.AllBlockCharacters.Contains(this))
        {
            BlockManager.Instance.AllBlockCharacters.Add(this);
        }
        BlockGoal.ListAllBlocks.Add(this);
        (GridCell _cell, float _distance) nearestGridCell = BlockManager.Instance.CheckNearestGrid(
        new GridHelper(transform.position.x, transform.position.z),
        null);
        AttachOnGrid(new BlockGridInfo(
            nearestGridCell._cell,
            nearestGridCell._distance
        ));
        featureInit();
        
    }
    void featureInit()
    {
        if (HorizontalVerticalFeature.Activate)
        {
            HorizontalVerticalFeature.HVGO.SetActive(true);
        }
    }
    void Update()
    {
        Debug.Log("UnattachChildren");
        // ChildBlock childBlock =  ChildBlocks[0].GetComponent<ChildBlock>();
        // childBlock.MainGridCell.GetGrid().OccupiedCharacter = null;
       
        
        // if (!BlockManager.Instance.IsDragging && !Multiply)
        // {
        //     if (MainGridCell.GetGrid() != null && MainGridCell.GetGrid().Grid != null)
        //     {
        //         var grid = MainGridCell.GetGrid();
        //         if (grid != null)
        //         {
        //             Vector3 snappedPos = new Vector3(grid.Grid.GetGridX(), transform.position.y, grid.Grid.GetGridZ());
        //             float snapThreshold = 0.1f;
        //             if (Vector3.Distance(transform.position, snappedPos) > snapThreshold)
        //             {
        //                 SnapPosition(grid);
        //             }
        //         }
        //     }
        // }  
        if (Multiply)
        {

            foreach (var child in ChildBlocks)
            {
                if (child.MainGridCell.GetGrid() != null)
                {
                    child.MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
                }
            }
        }
    }
    public override void AttachOnGrid(BlockGridInfo gridCell)
    {
       
        MainGridCell.SetGrid(gridCell.GetGrid());
        MainGridCell.SetDistanceOfGrid(gridCell.GetDistanceOfGrid());
    
        gridCell.GetGrid().AttachBlockCharacter(this);
        SnapPosition(gridCell.GetGrid());
        ChildrenAttachOnGrid();
        IsChildrenOnTargetGrid();
    


    }
    public void UnattachChildren()
    {
        foreach (var item in ChildBlocks)
        {
            if (item != null)
            {
                if (item.MainGridCell.GetGrid() != null && item.MainGridCell.GetGrid() != null)
                {
                    item.MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
                    Debug.Log("UnattachChildren");
                }

            }
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
        
        InitializeMultiply( );
    }
    public void InitializeMultiply()
    {
        Multiply = true;
        GetComponent<BoxCollider>().enabled = false;
        if (HorizontalVerticalFeature.Activate)
        {
            HorizontalVerticalFeature.HVGO.SetActive(false);
        }
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
            newChild.transform.SetParent(BattleController.Instance._LevelC.CurrentLevelObject.transform);
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
