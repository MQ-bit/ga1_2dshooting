using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const int DownwardEnemyIndex = 0;
    private const int AimedEnemyIndex = 1;
    private const int HomingEnemyIndex = 2;

    [Header("Spawn Timing")]
    [SerializeField, Min(0.01f)] private float _minimumSpawnInterval = 1f;
    [SerializeField, Min(0.01f)] private float _maximumSpawnInterval = 3f;

    [Header("Enemy Prefabs")]
    [Tooltip("Order: Downward, Aimed, Homing")]
    [SerializeField] private Enemy[] _enemyPrefabs;

    [Header("Spawn Chances")]
    [SerializeField, Range(0f, 100f)] private float _downwardEnemyChance = 50f;
    [SerializeField, Range(0f, 100f)] private float _aimedEnemyChance = 30f;

    private float _spawnInterval;
    private float _timer;

    private void Awake()
    {
        ScheduleNextSpawn();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _spawnInterval) return;

        _timer = 0f;
        Spawn();
        ScheduleNextSpawn();
    }

    private void Spawn()
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Length <= HomingEnemyIndex) return;

        float totalChance = _downwardEnemyChance + _aimedEnemyChance;
        float roll = Random.Range(0f, 100f);
        int enemyIndex;

        if (roll < _downwardEnemyChance)
        {
            enemyIndex = DownwardEnemyIndex;
        }
        else if (roll < totalChance)
        {
            enemyIndex = AimedEnemyIndex;
        }
        else
        {
            enemyIndex = HomingEnemyIndex;
        }

        Enemy prefab = _enemyPrefabs[enemyIndex];
        if (prefab != null) Instantiate(prefab, transform.position, Quaternion.identity);
    }

    private void ScheduleNextSpawn()
    {
        float minimum = Mathf.Min(_minimumSpawnInterval, _maximumSpawnInterval);
        float maximum = Mathf.Max(_minimumSpawnInterval, _maximumSpawnInterval);
        _spawnInterval = Random.Range(minimum, maximum);
    }

    private void OnValidate()
    {
        _aimedEnemyChance = Mathf.Min(_aimedEnemyChance, 100f - _downwardEnemyChance);
    }
}
