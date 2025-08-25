using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;
    public List<GridCell> AllGridCells = new List<GridCell>();
    public List<BlockCharacter> AllBlockCharacters = new List<BlockCharacter>();
    private Camera cam;
    public LayerMask blockCharacterLayer;
    public LayerMask groundGridLayer;
    [SerializeField]
    public bool IsDragging;
    public BlockDragHandler CurrentDraggingCharacter;
    private Vector3 dragOffset;
    void Awake()
    {
        cam = Camera.main;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        InitializeAllBlockChildren();  
    }
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, blockCharacterLayer) && Input.GetMouseButtonDown(0)
         && !IsDragging)
        {
            IsDragging = true;
            CurrentDraggingCharacter = hit.transform.GetComponent<BlockDragHandler>();

            Vector3 worldPoint = hit.point;
            dragOffset = CurrentDraggingCharacter.transform.position - new Vector3(worldPoint.x, CurrentDraggingCharacter.transform.position.y, worldPoint.z);

        }
        if (IsDragging && CurrentDraggingCharacter != null)
        {
            UnattachAllGrids();
            if (Physics.Raycast(ray, out RaycastHit dragHit, 100f, groundGridLayer))
            {
                Rigidbody rb = CurrentDraggingCharacter.GetComponent<Rigidbody>();
                float fixedY = rb.position.y;
                Vector3 targetPos = new Vector3(dragHit.point.x, fixedY, dragHit.point.z)  + dragOffset;

                rb.drag = 0;
                rb.mass = 0;
                if (rb != null)
                {
                    Vector3 direction = (targetPos - rb.position);
                    float speed = 50f;
                    rb.velocity = direction * speed;
                      // If colliding, slide along the surface
                     Vector3 velocity = direction * speed;

                    // probe forward; if we’ll hit something, slide along the surface
                    float rayLength = Mathf.Max(1f, velocity.magnitude * Time.fixedDeltaTime * 15f);

                    if (Physics.Raycast(rb.position, velocity.normalized, out RaycastHit hit2, rayLength, groundGridLayer))
                    {
                       
                        velocity = Vector3.ProjectOnPlane(velocity, hit2.normal);
                    }

                    rb.velocity = velocity;
                }
            }
           
        }
        if (IsDragging && Input.GetMouseButtonUp(0))
        {
            Rigidbody rb = CurrentDraggingCharacter.GetComponent<Rigidbody>();
            

            rb.velocity = Vector3.zero;
            (GridCell _cell, float _distance) nearestGridCell = CheckNearestGrid(new GridHelper(CurrentDraggingCharacter.transform.position.x, CurrentDraggingCharacter.transform.position.z),
                                                            CurrentDraggingCharacter.MainGridCell.GetGrid());

            bool sameParentGridCell = false;
            if (nearestGridCell._cell.OccupiedCharacter != null)
            {
                ChildBlock childBlock = nearestGridCell._cell.OccupiedCharacter.GetComponent<ChildBlock>();
                if (childBlock != null && childBlock.Parent == CurrentDraggingCharacter)
                {
                    sameParentGridCell = true;
                }
            }
            if (nearestGridCell._cell.OccupiedCharacter == null || sameParentGridCell)
            {

                GridCell newGrid = nearestGridCell._cell;
                CurrentDraggingCharacter.LastBlockGridInfo = CurrentDraggingCharacter.MainGridCell;
                //UnattachAllGrids();
                //CurrentDraggingCharacter.SnapPosition(newGrid);

                bool childSameGridCellOrDistanceIssue = false;
                if (AllBlockCharactersCanFit(CurrentDraggingCharacter) 
                    )
                {
                    nearestGridCell = CheckNearestGrid(new GridHelper(CurrentDraggingCharacter.transform.position.x,
                                                                    CurrentDraggingCharacter.transform.position.z),
                                                                    CurrentDraggingCharacter.MainGridCell.GetGrid());

                    CurrentDraggingCharacter.AttachOnGrid(new BlockGridInfo
                    (
                        nearestGridCell._cell,
                        nearestGridCell._distance
                    ) );

                    foreach (var child in CurrentDraggingCharacter.ChildBlocks)
                    {
                        if (IsSameGridOrDistanceIssue(CurrentDraggingCharacter, child))
                        {
                            childSameGridCellOrDistanceIssue = true;
                            break;
                        }
                    }
                    
                    if (childSameGridCellOrDistanceIssue)
                    {
                        CurrentDraggingCharacter.AttachOnGrid(CurrentDraggingCharacter.LastBlockGridInfo);
                    }
                }
                else
                {
                    CurrentDraggingCharacter.AttachOnGrid(CurrentDraggingCharacter.LastBlockGridInfo);

                }
                
            }
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.mass = 500;
            rb.drag = 500;
            IsDragging = false;
            CurrentDraggingCharacter = null;
        }
    }
    public void UnattachAllGrids()
    {
        if (CurrentDraggingCharacter.MainGridCell.GetGrid() != null)
        {
            CurrentDraggingCharacter.MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
        }
        foreach (var child in CurrentDraggingCharacter.ChildBlocks)
        {
            if (child.MainGridCell.GetGrid() != null)
            {
                child.MainGridCell.GetGrid().UnattachCurrentBlockCharacter();
            }
        }
    }
    public bool IsSameGridOrDistanceIssue(BlockCharacter parentCharacter, BlockCharacter childCharacter)
    {
        foreach (var child in parentCharacter.GetComponent<BlockDragHandler>().ChildBlocks)
        {
            if (child != childCharacter && childCharacter.MainGridCell.GetGrid() == child.MainGridCell.GetGrid())
            {
                return true;
            }

            else
            {
                (GridCell _cell, float _distance) nearestGridCell = CheckNearestGrid(new GridHelper(child.transform.position.x,
                                                                                            child.transform.position.z),
                                                                                            child.MainGridCell.GetGrid());
                if (nearestGridCell._distance >= 0.90)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public (GridCell _cell, float _distance) CheckNearestGrid(GridHelper gridHelper, GridCell currentGrid)
    {
      
        Vector3 targetPos = new Vector3(gridHelper.GetGridX(), 0, gridHelper.GetGridZ());

        GridCell nearestCell = null;
        float nearestDistanceNewGrid = float.MaxValue;

        foreach (var cell in AllGridCells)
        {
            if (cell == currentGrid)
                continue;

            Vector3 cellPos = new Vector3(cell.Grid.GetGridX(), 0, cell.Grid.GetGridZ());
            float distance = Vector3.Distance(cellPos, targetPos); // world distance

            if (distance < nearestDistanceNewGrid)
            {
                nearestDistanceNewGrid = distance;
                nearestCell = cell;
            }
        }

        // If no current grid, just return the closest one
        if (currentGrid == null)
            return (nearestCell, nearestDistanceNewGrid);

        // Otherwise, compare to current grid
        Vector3 currentPos = new Vector3(currentGrid.Grid.GetGridX(), 0, currentGrid.Grid.GetGridZ());
        float currentDistanceOldGrid = Vector3.Distance(currentPos, targetPos);

        return (nearestDistanceNewGrid < currentDistanceOldGrid) 
            ? (nearestCell, nearestDistanceNewGrid) 
            : (currentGrid, currentDistanceOldGrid);
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && CurrentDraggingCharacter != null)
        {
               BoxCollider box = CurrentDraggingCharacter.GetComponent<BoxCollider>();

                Vector3 worldCenter = box.transform.TransformPoint(box.center);
                Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);

                Gizmos.matrix = Matrix4x4.TRS(worldCenter, box.transform.rotation, Vector3.one);

                // Fill
                Gizmos.color = new Color(0, 1, 0, 0.15f);
                Gizmos.DrawCube(Vector3.zero, halfExtents * 2);

                // Borders (stronger)
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
        }
    }
    public bool AllBlockCharactersCanFit(BlockDragHandler blockDragHandler)
    {
        GridCell blockParentGridCell = CheckNearestGrid(
            new GridHelper(blockDragHandler.transform.position.x,
                            blockDragHandler.transform.position.z),
                            blockDragHandler.MainGridCell.GetGrid())._cell;
        if (blockParentGridCell.OccupiedCharacter)
        {
            return false;
        }
        foreach (var child in blockDragHandler.ChildBlocks)
        {
            GridCell gridChildCell = CheckNearestGrid(
            new GridHelper(child.transform.position.x,
                            child.transform.position.z),
                            child.MainGridCell.GetGrid())._cell;
            if (gridChildCell.OccupiedCharacter)
            {
                return false;
            }
        }
        return true;
    }
   
    public void InitializeAllBlockChildren()
    {
        foreach (var item in AllBlockCharacters)
        {
            (GridCell _cell, float _distance) nearestGridCell = BlockManager.Instance.CheckNearestGrid(
            new GridHelper(item.transform.position.x, item.transform.position.z),
            null);
            item.AttachOnGrid(new BlockGridInfo(
                nearestGridCell._cell,
                nearestGridCell._distance
            ));
        }
    }

}

