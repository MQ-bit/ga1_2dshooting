using UnityEngine;

public class HomingEnemy : Enemy
{
    // 캐싱: 자주 쓸법한 데이터(객체)를 가까운 곳에 저장해두고 쓰는거
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    protected override void Move()
    {
        if (_player == null) return;

        // 1. 방향을 구한다.
        Vector2 direction = _player.transform.position - transform.position;
        direction.Normalize();

        // 2. 방향과 속도에 맞게 이동한다.
        transform.Translate(direction * _moveSpeed * Time.deltaTime);
    }
}