using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //필요 속성
    //-타이머
    [Header("스폰 간격")] [SerializeField] private float _spawnInterval = 3f;
    private float _timer;
    
    // -생성형 프리퀄
    
    [Header("스폰할 적 프리펩")] [SerializeField] private Enemy _enemyPrefab;
    private void Start()
    {
        
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0;

            _spawnInterval = Random.Range(1f, 3f);
            Spawn();
        }

    }

    private void Spawn()
    {
        Enemy enemy = Instantiate(_enemyPrefab);
        enemy.transform.position = transform.position;
    }
}

