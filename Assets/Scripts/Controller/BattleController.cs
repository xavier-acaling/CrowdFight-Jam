using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class BattleController : MonoBehaviour
{
    // Start is called before the first frame update
    public static BattleController Instance;
    public List<Enemy> AllEnemies = new List<Enemy>();
    public GameObject PrefabSplash;
    public Text EnemyCountUI;
    public Text RestartUI;
    public GameObject GameOverUI;
    public int AllEnemiesCount;
    public LevelController _LevelC;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
     //  AllEnemiesCount = AllEnemies.Count;
        Instance = this;
      //  UpdateGame();
    }
    void Start()
    {

    }
    
    public void UpdateGame()
    {
        EnemyCountUI.text = AllEnemiesCount.ToString();
        if (AllEnemiesCount == 0)
        {
            GameOverUI.SetActive(true);
            StartCoroutine(Next());
        }
    }
    public IEnumerator Next()
    {
        int count = 3;
        while (count > 0)
        {
            count--;
            RestartUI.text = $"NEXT LEVEL IN {count}";
            yield return new WaitForSeconds(1);
        }
        BattleController.Instance._LevelC.NextLevel();
        GameOverUI.SetActive(false);
    } 
    public void FindEnemy(ChildBlock child)
    {
        Enemy enemy = AllEnemies.FirstOrDefault(w => w.TargetCharacter == null);
        if (enemy != null)
        {
            enemy.TargetCharacter = child;
            child.TargetEnemy = enemy;
            AllEnemies.Remove(enemy);
            StartCoroutine(battleStart(enemy, child));

        }
    }
    IEnumerator battleStart(Enemy enemy,ChildBlock child)
    {
        enemy.Anim.SetBool("run",true);
        child.Anim.SetBool("run",true);
        bool childCompletePath = false;

        if (child.Parent.BlockGoal.IntroPath.Count <  1)
        {
            childCompletePath = true;
        }
        else
        {
            child.transform.DOPath(child.Parent.BlockGoal.IntroPath.Select(w => w.position).ToArray(),child.Parent.BlockGoal.IntoPathSeconds,PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() =>
            {
                childCompletePath = true;
            });
        }
        while (true)
        {
            if (enemy == null && child == null)
            {
                yield break;
            }
            float distance = Vector3.Distance(new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z),
                                              new Vector3(child.transform.position.x, 0, child.transform.position.z));

            Vector3 midpoint = (enemy.transform.position + child.transform.position) / 2f;

          //  enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, midpoint, 2f * Time.deltaTime);
            if (childCompletePath)
            {
                child.transform.position = Vector3.MoveTowards(child.transform.position, midpoint, 2f * Time.deltaTime);
                
            }
            //Vector3 enemyLookDir = child.transform.position - enemy.transform.position;
            //  enemyLookDir.y = 0; 
            // if (enemyLookDir != Vector3.zero)
            // {
            //     Quaternion targetRot = Quaternion.LookRotation(enemyLookDir);
            //     enemy.transform.rotation = Quaternion.Slerp(
            //         enemy.transform.rotation,
            //         targetRot,
            //         Time.deltaTime * 5f
            //     );
            // }

            Vector3 childLookDir = enemy.transform.position - child.transform.position;
            childLookDir.y = 0;
            if (childLookDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(childLookDir);
                child.transform.rotation = Quaternion.Slerp(
                    child.transform.rotation,
                    targetRot,
                    Time.deltaTime * 5f
                );
            }
            if (distance <= 0.4f)
            {
                AllEnemiesCount -= 1;
                AllEnemiesCount = Mathf.Clamp(AllEnemiesCount, 0, int.MaxValue);
                UpdateGame();

                GameObject fx = Instantiate(PrefabSplash, new Vector3(enemy.transform.position.x, PrefabSplash.transform.position.y, enemy.transform.position.z), Quaternion.identity);
    
                Destroy(enemy.gameObject);
                Destroy(child.gameObject);
                yield break;
            }
            yield return null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
