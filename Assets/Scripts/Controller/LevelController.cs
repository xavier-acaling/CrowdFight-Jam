using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<LevelSO> Levels = new List<LevelSO>();
    public int CurrentLevel = 1;
    public Text LevelUI;
    public event Action OnLevelUpdate;
    public Transform LevelContainer;
    public GameObject CurrentLevelObject;
  
    void Start()
    {
        OnLevelUpdate?.Invoke();
    }
    void OnEnable()
    {
        OnLevelUpdate += spawnLevel;
        OnLevelUpdate += LevelUIUpdate;
    }
    void OnDisable()
    {
        OnLevelUpdate -= spawnLevel;
        OnLevelUpdate -= LevelUIUpdate;
    }
    void LevelUIUpdate()
    {
        LevelUI.text = $"LEVEL {CurrentLevel}";
    }
    void init()
    {
        
    }
    void spawnLevel()
    {
        if (CurrentLevelObject != null)
        {
            Destroy(CurrentLevelObject);
        }
        BlockManager.Instance.AllBlockCharacters.Clear();
        BlockManager.Instance.AllGridCells.Clear();
        LevelSO levelSO = Levels[CurrentLevel - 1];
        GameObject level = Instantiate(levelSO.LevelPrefab.gameObject, LevelContainer);
        CurrentLevelObject = level;
        BlockManager.Instance.InitializeAllBlockChildren();
    }
    [ContextMenu("RestartLevel")]
    public void RestartLevel()
    {
        OnLevelUpdate?.Invoke();
    }
    [ContextMenu("NextLevel")]
    public void NextLevel()
    {
        CurrentLevel++;
        CurrentLevel = Mathf.Clamp(CurrentLevel, 1, Levels.Count);
        OnLevelUpdate?.Invoke();
    }
    [ContextMenu("PrevLevel")]
    public void PrevLevel()
    {
        CurrentLevel--;
        CurrentLevel = Mathf.Clamp(CurrentLevel, 1, Levels.Count);
        OnLevelUpdate?.Invoke();
    }
    public void SaveLevel()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PrevLevel();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextLevel();
        }
    }
}
