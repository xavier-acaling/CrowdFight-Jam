using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockCharacter : MonoBehaviour
{
    public BlockGridInfo MainGridCell;
    public abstract void AttachOnGrid(BlockGridInfo gridCell);
}
