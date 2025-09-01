using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    public LevelSO LevelSO;
    public List<Enemy> InitAllEnemies = new List<Enemy>();
    public int InitAllEnemiesCount;
    // Start is called before the first frame update

    public Transform ContainerEnemies;
    private Vector3 baseContainerPos;
    public bool Dev;
    void Start()
    {

         InitAllEnemies.Clear();

        // Spawn enemies dynamically
        for (int i = 0; i < LevelSO.EnemyCount; i++)
        {
            GameObject obj = Instantiate(BattleController.Instance.EnemyPrefab, ContainerEnemies);
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.transform.position = Vector3.zero;
                InitAllEnemies.Add(enemy);
            }
        }
        BattleController.Instance.AllEnemiesCount = InitAllEnemies.Count;
        BattleController.Instance.AllEnemies = InitAllEnemies;
        BattleController.Instance.UpdateGame();
        baseContainerPos = ContainerEnemies.position;
        ArrangeEnemies();
    }
    void Update()
    {
        if (!BattleController.Instance.GameStarted)
        {
            ArrangeEnemies();
        }
    }

    [ContextMenu("Arrange Enemies")]
    public void ArrangeEnemies()
    {
        if (InitAllEnemies.Count == 0) return;

        switch (LevelSO.formation)
        {
            case FormationType.Square: ArrangeSquare(); break;
            case FormationType.Rectangle: ArrangeRectangle(); break;
            case FormationType.Circle: ArrangeCircle(); break;
            case FormationType.Ellipse: ArrangeEllipse(); break;
            case FormationType.Triangle: ArrangeTriangle(); break;
            case FormationType.Diamond: ArrangeDiamond(); break;
            case FormationType.Hexagon: ArrangeHexagon(); break;
            case FormationType.Line: ArrangeLine(); break;
            case FormationType.Cross: ArrangeCross(); break;
            case FormationType.Heart: ArrangeHeart(); break;
            case FormationType.Star: ArrangeStar(); break;
            case FormationType.Spiral: ArrangeSpiral(); break;
            case FormationType.VShape: ArrangeVShape(); break;
            case FormationType.UShape: ArrangeUShape(); break;
            case FormationType.Arrow: ArrangeArrow(); break;
            case FormationType.Wave: ArrangeWave(); break;
            case FormationType.Infinity: ArrangeInfinity(); break;
            case FormationType.Pentagon: ArrangePolygon(5); break;
            case FormationType.Octagon: ArrangePolygon(8); break;
            case FormationType.SnakeLine: ArrangeSnakeLine(); break;
        }
        Vector3 pos = baseContainerPos + new Vector3(LevelSO.PositionOffset.x, 0, LevelSO.PositionOffset.y);
        ContainerEnemies.position = pos;
    }

    // --- Existing shapes ---

    private void ArrangeSquare()
    {
        int count = InitAllEnemies.Count;
        int side = Mathf.CeilToInt(Mathf.Sqrt(count));
        int index = 0;

        float offset = (side - 1) * LevelSO.spacing * 0.5f;

        for (int x = 0; x < side && index < count; x++)
        {
            for (int z = 0; z < side && index < count; z++)
            {
                InitAllEnemies[index].transform.localPosition =
                    new Vector3(x * LevelSO.spacing - offset, 0, z * LevelSO.spacing - offset);
                index++;
            }
        }
    }

    private void ArrangeRectangle()
    {
        int count = InitAllEnemies.Count;
        int width = Mathf.CeilToInt(Mathf.Sqrt(count) * 1.5f);
        int height = Mathf.CeilToInt((float)count / width);
        int index = 0;

        float offsetX = (width - 1) * LevelSO.spacing * 0.5f;
        float offsetZ = (height - 1) * LevelSO.spacing * 0.5f;

        for (int x = 0; x < width && index < count; x++)
        {
            for (int z = 0; z < height && index < count; z++)
            {
                InitAllEnemies[index].transform.localPosition =
                    new Vector3(x * LevelSO.spacing - offsetX, 0, z * LevelSO.spacing - offsetZ);
                index++;
            }
        }
    }

    private void ArrangeCircle()
    {
        int count = InitAllEnemies.Count;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * LevelSO.radius;
            float z = Mathf.Sin(angle) * LevelSO.radius;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void ArrangeEllipse()
    {
        int count = InitAllEnemies.Count;
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * LevelSO.radius * 1.5f;
            float z = Mathf.Sin(angle) * LevelSO.radius;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void ArrangeTriangle()
    {
        int count = InitAllEnemies.Count;
        int rows = Mathf.CeilToInt(Mathf.Sqrt(count * 2));
        int index = 0;

        for (int row = 0; row < rows && index < count; row++)
        {
            int enemiesInRow = row + 1;
            float offset = -(enemiesInRow - 1) * LevelSO.spacing * 0.5f;

            for (int col = 0; col < enemiesInRow && index < count; col++)
            {
                float x = offset + col * LevelSO.spacing;
                float z = -row * LevelSO.spacing;
                InitAllEnemies[index].transform.localPosition = new Vector3(x, 0, z);
                index++;
            }
        }
    }

    private void ArrangeDiamond()
    {
        int count = InitAllEnemies.Count;
        int size = Mathf.CeilToInt(Mathf.Sqrt(count));
        int index = 0;

        for (int row = -size; row <= size && index < count; row++)
        {
            int width = size - Mathf.Abs(row);
            for (int col = -width; col <= width && index < count; col++)
            {
                InitAllEnemies[index].transform.localPosition = new Vector3(col * LevelSO.spacing, 0, row * LevelSO.spacing);
                index++;
            }
        }
    }

    private void ArrangeHexagon()
    {
        int count = InitAllEnemies.Count;
        int index = 0;
        int radiusHex = Mathf.CeilToInt(Mathf.Sqrt(count / 3));

        for (int q = -radiusHex; q <= radiusHex && index < count; q++)
        {
            int r1 = Mathf.Max(-radiusHex, -q - radiusHex);
            int r2 = Mathf.Min(radiusHex, -q + radiusHex);

            for (int r = r1; r <= r2 && index < count; r++)
            {
                float x = LevelSO.spacing * (q + r / 2f);
                float z = LevelSO.spacing * (r * Mathf.Sqrt(3) / 2f);
                InitAllEnemies[index].transform.localPosition = new Vector3(x, 0, z);
                index++;
            }
        }
    }

    private void ArrangeLine()
    {
        float offset = (InitAllEnemies.Count - 1) * LevelSO.spacing * 0.5f;
        for (int i = 0; i < InitAllEnemies.Count; i++)
        {
            InitAllEnemies[i].transform.localPosition = new Vector3(i * LevelSO.spacing - offset, 0, 0);
        }
    }

    private void ArrangeCross()
    {
        int count = InitAllEnemies.Count;
        int half = count / 2;
        int index = 0;

        for (int i = -half / 2; i <= half / 2 && index < count; i++)
        {
            InitAllEnemies[index].transform.localPosition = new Vector3(i * LevelSO.spacing, 0, 0);
            index++;
        }

        for (int i = -half / 2; i <= half / 2 && index < count; i++)
        {
            InitAllEnemies[index].transform.localPosition = new Vector3(0, 0, i * LevelSO.spacing);
            index++;
        }
    }

    private void ArrangeHeart()
    {
        int count = InitAllEnemies.Count;
        float scale = LevelSO.radius * 0.1f;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count * Mathf.PI * 2f;

            float x = 16 * Mathf.Pow(Mathf.Sin(t), 3) * scale;
            float z = (13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t)) * scale;

            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    // --- New Shapes ---

    private void ArrangeStar()
    {
        int count = InitAllEnemies.Count;
        float outer = LevelSO.radius;
        float inner = LevelSO.radius * 0.4f;

        for (int i = 0; i < count; i++)
        {
            float angle = (float)i / count * Mathf.PI * 2f;
            float r = (i % 2 == 0) ? outer : inner;
            float x = Mathf.Cos(angle) * r;
            float z = Mathf.Sin(angle) * r;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void ArrangeSpiral()
    {
        int count = InitAllEnemies.Count;
        float angle = 0f;
        float r = LevelSO.spacing;

        for (int i = 0; i < count; i++)
        {
            float x = Mathf.Cos(angle) * r;
            float z = Mathf.Sin(angle) * r;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);

            angle += 0.5f;
            r += LevelSO.spacing * 0.1f;
        }
    }

    private void ArrangeVShape()
    {
        int count = InitAllEnemies.Count;
        int half = count / 2;
        int index = 0;

        for (int i = 0; i < half; i++)
        {
            float offset = i * LevelSO.spacing;
            InitAllEnemies[index++].transform.localPosition = new Vector3(offset, 0, -offset);
            if (index < count)
                InitAllEnemies[index++].transform.localPosition = new Vector3(-offset, 0, -offset);
        }
    }

    private void ArrangeUShape()
    {
        int count = InitAllEnemies.Count;
        int height = Mathf.CeilToInt(count / 10f);
        int index = 0;

        for (int y = 0; y < height && index < count; y++)
        {
            InitAllEnemies[index++].transform.localPosition = new Vector3(0, 0, -y * LevelSO.spacing);
            if (index < count)
                InitAllEnemies[index++].transform.localPosition = new Vector3(10 * LevelSO.spacing, 0, -y * LevelSO.spacing);
        }

        for (int x = 0; x <= 10 && index < count; x++)
        {
            InitAllEnemies[index++].transform.localPosition = new Vector3(x * LevelSO.spacing, 0, -height * LevelSO.spacing);
        }
    }

    private void ArrangeArrow()
    {
        int count = InitAllEnemies.Count;
        int index = 0;
        int width = 7;

        for (int y = 0; y < count / width; y++)
        {
            for (int x = -y; x <= y && index < count; x++)
            {
                InitAllEnemies[index++].transform.localPosition = new Vector3(x * LevelSO.spacing, 0, -y * LevelSO.spacing);
            }
        }
    }

    private void ArrangeWave()
    {
        int count = InitAllEnemies.Count;

        for (int i = 0; i < count; i++)
        {
            float x = i * LevelSO.spacing * 0.5f;
            float z = Mathf.Sin(i * 0.5f) * LevelSO.spacing * 3f;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void ArrangeInfinity()
    {
        int count = InitAllEnemies.Count;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count * Mathf.PI * 2f;
            float x = Mathf.Sin(t) * LevelSO.radius;
            float z = Mathf.Sin(t * 2) * LevelSO.radius * 0.5f;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void ArrangePolygon(int sides)
    {
        int count = InitAllEnemies.Count;
        float angleStep = 360f / sides;

        for (int i = 0; i < count; i++)
        {
            int seg = i % sides;
            float angle = seg * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * LevelSO.radius;
            float z = Mathf.Sin(angle) * LevelSO.radius;
            InitAllEnemies[i].transform.localPosition = new Vector3(x, 0, z);
        }
    }

    private void ArrangeSnakeLine()
    {
        int count = InitAllEnemies.Count;
        int width = 10;
        int index = 0;

        for (int y = 0; y < count / width; y++)
        {
            for (int x = 0; x < width && index < count; x++)
            {
                float posX = (y % 2 == 0) ? x : width - x - 1;
                InitAllEnemies[index++].transform.localPosition = new Vector3(posX * LevelSO.spacing, 0, y * LevelSO.spacing);
            }
        }
    }
}
