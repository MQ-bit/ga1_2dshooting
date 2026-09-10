using UnityEngine;

public class PlayerAutoMove : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float _enemyRefreshInterval = 0.3f;

    private Enemy[] _enemies;
    private float _refreshTimer;

    private void Start()
    {
        RefreshEnemies();
    }

    private void Update()
    {
        _refreshTimer += Time.deltaTime;
        if (_refreshTimer < _enemyRefreshInterval) return;

        _refreshTimer = 0f;
        RefreshEnemies();
    }

    private void RefreshEnemies()
    {
        _enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }
}
