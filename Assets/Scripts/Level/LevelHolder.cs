using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    public List<Enemy> InitAllEnemies = new List<Enemy>();
    public int InitAllEnemiesCount;
    // Start is called before the first frame update
    void Start()
    {
        BattleController.Instance.AllEnemiesCount = InitAllEnemies.Count;
        BattleController.Instance.AllEnemies = InitAllEnemies;
        BattleController.Instance.UpdateGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
