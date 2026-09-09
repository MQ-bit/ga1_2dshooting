using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _spawnInterval = 3f;
    [SerializeField] private Enemy[] _enemyPrefabs;
    [SerializeField, Range(0f, 1f)] private float _laneJitter = 0.22f;
    private float _timer;
    private float _nextInterval;

    private void Start()
    {
        // Stagger the lanes so the opening wave does not arrive as a wall.
        _nextInterval = Mathf.Max(0.5f, _spawnInterval) + Random.Range(0f, 1.4f);
    }

    private void Update()
    {
        if (GameSession.InputBlocked) return;
        _timer += Time.deltaTime;
        if (_timer < _nextInterval) return;
        _timer = 0f;
        int sector = GameSession.Instance != null ? GameSession.Instance.Sector : 1;
        float pressure = Mathf.Min(2.2f, 1f + (sector - 1) * 0.12f);
        _nextInterval = Random.Range(1.5f, 3f) / pressure;
        Spawn();
    }

    private void Spawn()
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Length == 0) return;
        int roll = Random.Range(0, 100);
        int index = roll < 50 ? 0 : roll < 80 ? 1 : 2;
        index = Mathf.Min(index, _enemyPrefabs.Length - 1);
        Enemy prefab = _enemyPrefabs[index];
        if (prefab == null) return;
        Vector3 position = transform.position + Vector3.right * Random.Range(-_laneJitter, _laneJitter);
        Enemy enemy = Instantiate(prefab, position, Quaternion.identity);
        // A missed homing enemy must not remain alive indefinitely.
        Destroy(enemy.gameObject, 18f);
    }
}