using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Game/Level")]
public class LevelSO : ScriptableObject
{
    [Header("Level")]
    public LevelHolder LevelPrefab;
    [Header("Enemy Formataion")]
    public float spacing = 2f;    
    public float radius = 10f; 
    public float scale = 10f;
    public FormationType formation = FormationType.Square;
    public Vector2 PositionOffset;
    public int EnemyCount = 20; 
}
