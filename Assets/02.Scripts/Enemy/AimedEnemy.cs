using UnityEngine;

public class AimedEnemy : Enemy
{
    private Animator _animator;
    private GameObject _player;
    private Vector2 _direction;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    protected override void OnHit()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Hit");
    }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        if (_player == null)
        {
            // 플레이어가 사라졌다면 방향 계산을 하지 않는다.
            return;
        }

        _direction = _player.transform.position - transform.position;
        _direction.Normalize();

        // 시작할 때 한 번 플레이어를 바라본다. 스프라이트의 정면은 아래쪽이다.
        if (_direction.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
    }

    protected override void Move()
    {
        //  방향과 속도에 맞게 이동한다.
        transform.Translate(_direction * _moveSpeed * Time.deltaTime, Space.World);
    }
}
