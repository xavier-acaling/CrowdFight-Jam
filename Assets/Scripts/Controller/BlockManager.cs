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
        //   InitializeAllBlockChildren();  
        
    }
    void Update()
    {
        if (IsDragging && CurrentDraggingCharacter == null)
        {
            IsDragging = false;
        }
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
                if (rb == null) return;

                float fixedY = rb.position.y;
                Vector3 targetPos = new Vector3(dragHit.point.x, fixedY, dragHit.point.z) + dragOffset;

                rb.mass = 0f;
                rb.drag = 0f;
                rb.angularDrag = 0f;

                Vector3 direction = targetPos - rb.position;
                float distance = direction.magnitude;
                direction.Normalize();

                float speed = 50f;
                Vector3 desiredVelocity = direction * Mathf.Min(speed, distance / Time.fixedDeltaTime);

                // Short raycast for collisions
                float rayLength = Mathf.Max(0.01f, desiredVelocity.magnitude * Time.fixedDeltaTime);
                if (Physics.Raycast(rb.position, desiredVelocity.normalized, out RaycastHit hit2, rayLength, groundGridLayer))
                {
                    if (hit2.transform != CurrentDraggingCharacter.transform) // ignore self
                    {
                        desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, hit2.normal);
                    }
                }


                Vector3 velocity = Vector3.Lerp(rb.velocity, desiredVelocity, 0.05f);

                if (distance < 0.25f)
                {
                    velocity = Vector3.Lerp(velocity, Vector3.zero, 1);
                }

                if (velocity.magnitude < 3f && distance > 0.3f)
                {
                    velocity = velocity.normalized * 3f;
                }


                float probeDist = 1f; 
                Vector3 probePos = rb.position + velocity.normalized * probeDist;

                if (Physics.Raycast(probePos + Vector3.up * 2f, Vector3.down, out RaycastHit stepHit, 5f, groundGridLayer))
                {
                    float heightDiff = rb.position.y - stepHit.point.y;

                    if (heightDiff > 0.1f && heightDiff < 5f)
                    {
                        velocity = velocity.normalized * velocity.magnitude;
                    }
                }


                rb.velocity = velocity;

                if (CurrentDraggingCharacter.HorizontalVerticalFeature.Activate)
                {
                    switch (CurrentDraggingCharacter.HorizontalVerticalFeature.Type)
                    {
                        case TypeHV.Horizontal:
                            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
                            break;
                        case TypeHV.Vertical:
                            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
                            break;
                    }
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
                if (nearestGridCell._distance >= 2f)
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

        return (nearestDistanceNewGrid + 0.35f < currentDistanceOldGrid) 
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

