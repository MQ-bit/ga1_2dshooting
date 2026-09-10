using UnityEngine;

public class PlayerAutoMove : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float _enemyRefreshInterval = 0.3f;
    [SerializeField, Min(0f)] private float _moveSpeed = 3f;
    [SerializeField, Min(0.1f)] private float _detectRadius = 5f;

    private Enemy[] _enemies;
    private float _refreshTimer;

    private void Start()
    {
        RefreshEnemies();
    }

    private void Update()
    {
        _refreshTimer += Time.deltaTime;
        if (_refreshTimer >= _enemyRefreshInterval)
        {
            _refreshTimer = 0f;
            RefreshEnemies();
        }
 
        MoveAwayFromEnemies();
    }

    private void RefreshEnemies()
    {
        _enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    private void MoveAwayFromEnemies()
    {
        Vector2 avoidDirection = Vector2.zero;

        foreach (Enemy enemy in _enemies)
        {
            if (enemy == null) continue;

            Vector2 away = (Vector2)transform.position - (Vector2)enemy.transform.position;
            float distance = away.magnitude;
            if (distance > _detectRadius) continue;

            // 가까운 적일수록 더 큰 영향을 준다.
            avoidDirection += away.normalized / Mathf.Max(distance, 0.1f);
        }

        transform.Translate(avoidDirection.normalized * (_moveSpeed * Time.deltaTime));
    }
}
