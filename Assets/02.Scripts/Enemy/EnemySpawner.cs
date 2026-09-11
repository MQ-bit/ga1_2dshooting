using UnityEngine;

// 역할: 일정 시간마다 적을 생성한다.
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private EnemySpawnDataTableSO _spawnDataTable;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0f;
            _spawnInterval = Random.Range(1f, 3f);

            Spawn();
        }
    }

    private void Spawn()
    {
        // 추첨할 수 있는 모든 가중치를 더한다.
        int totalWeight = 0;
        foreach (EnemySpawnData data in _spawnDataTable.Datas)
        {
            totalWeight += data.Percent;
        }

        // 전체 가중치 범위에서 랜덤한 정수를 뽑는다.
        int randomWeight = Random.Range(0, totalWeight);

        // 가중치를 누적하면서 선택된 구간을 찾는다.
        int cumulativeWeight = 0;
        foreach (EnemySpawnData data in _spawnDataTable.Datas)
        {
            cumulativeWeight += data.Percent;
            if (randomWeight < cumulativeWeight)
            {
                GameObject enemy = Instantiate(data.EnemyPrefab);
                enemy.transform.position = transform.position;
                break;
            }
        }
    }
}
