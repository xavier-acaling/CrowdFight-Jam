using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    public int CurrentLevelIndex;
    void Start()
    {
        init();
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
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("data")))
        {
            string data = PlayerPrefs.GetString("data");
            GameData gameData = JsonConvert.DeserializeObject<GameData>(data);
            CurrentLevel = gameData.LevelCount;
            CurrentLevelIndex = gameData.LevelIndex;
        }
        else
        {
            saveLevel();
        }
        OnLevelUpdate?.Invoke();
    }
    void saveLevel()
    {
        GameData gameData = new GameData
        {
            LevelCount = CurrentLevel,
            LevelIndex = CurrentLevelIndex
        };
        string data = JsonConvert.SerializeObject(gameData);

        PlayerPrefs.SetString("data", data);
        PlayerPrefs.Save();
    }
    void spawnLevel()
    {
        if (CurrentLevelObject != null)
        {
            Destroy(CurrentLevelObject);
        }
        BattleController.Instance.GameStarted = false;
        BlockManager.Instance.AllBlockCharacters.Clear();
        BlockManager.Instance.AllGridCells.Clear();

        
        LevelSO levelSO = Levels[CurrentLevelIndex];

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
        CurrentLevelIndex = 0;
        CurrentLevelIndex = CurrentLevel - 1;
        if (CurrentLevel > 14)
        {
            CurrentLevelIndex = UnityEngine.Random.Range(1, 15);
            CurrentLevelIndex--;
        }
        saveLevel();
        
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
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     PrevLevel();
        // }
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
