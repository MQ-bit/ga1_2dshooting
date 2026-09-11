using UnityEngine;

public class PlayerAutoMove : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _stopTrackingY = 2;

    private GameObject _target = null;

    private void Update()
    {
        if (_target == null || _target.transform.position.y < -_stopTrackingY)
        {
            FindNearestTarget();
        }

        Move();
    }

    private void Move()
    {
        if (_target == null) return;

        // 2. 방향을 구한다.
        Vector3 diff = _target.transform.position - transform.position;
        Vector3 direction = diff;

        // 적과 나와의 y축 차이가 3보다 크면 앞으로 가고 아니라면 뒤로가게
        if (diff.y >= 3)
        {
            direction.y = 1;
        }
        else
        {
            direction.y = -1;
        }

        direction.Normalize();

        // 3. 속도에 맞게 이동을한다.
        transform.position += direction * _speed * Time.deltaTime;
    }

    private void FindNearestTarget()
    {
        // 1. 타겟을 구한다.
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        if (targets.Length == 0) return;

        _target = targets[0];
        float minDistance = float.MaxValue;

        // 1-1. 가장 가까운 타겟을 찾는다.
        foreach (GameObject enemy in targets)
        {
            if (enemy.transform.position.y < -_stopTrackingY)
            {
                continue;
            }

            // 거리를 구해서
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance) // 저장된 거리보다 짧다면
            {
                // 타겟 변경
                minDistance = distance;
                _target = enemy;
            }
        }
    }
}
