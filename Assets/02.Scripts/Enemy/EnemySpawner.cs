using UnityEngine;

// 역할: 일정 시간마다 적을 생성해주고 싶다.
public class EnemySpawner : MonoBehaviour
{
    // 필요 속성
    // - 타이머
    [SerializeField] private float _spawnInterval = 3f;

    private float _timer;

    // - 생성할 프리팹들
    [SerializeField] private Enemy[] _enemyPrefabs;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            _timer = 0;

            _spawnInterval = Random.Range(1f, 3f); // float: 1 ~ 3

            Spawn();
        }
    }

    private void Spawn()
    {
        // 각 스포너가 적을 스폰할때 확률에 따라 다른 타입의 적을 스폰해주세요.
        // 50%: [0] Downward
        // 30%: [1] Aimed
        // 20%: [2] Homing

        int enemyPrefabIndex = 0;
        int radomPercent = UnityEngine.Random.Range(0, 100);

        // Todo: Scriptable Object를 사용해서 리팩토링
        // 이유 1: 배열을 사용했지만 각 아이템이 어떤 프리팹인지 알수가 없음
        // 이유 2: 각 에너미 스폰 확률을 매직 넘버로 하드코딩해서 유지보수가 어렵
        if (radomPercent < 50)
        {
            enemyPrefabIndex = 0;
        }
        else if (radomPercent < 80)
        {
            enemyPrefabIndex = 1;
        }
        else
        {
            enemyPrefabIndex = 2;
        }


        Enemy enemy = Instantiate(_enemyPrefabs[enemyPrefabIndex]);
        enemy.transform.position = transform.position;
    }
}