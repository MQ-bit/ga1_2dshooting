using UnityEngine;

public class AimedEnemy : Enemy
{
    private Animator _animator;
    private GameObject _player;
    private Vector2 _direction;

    protected override void Awake()
    {
        base.Awake();
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
            return;
        }

        _direction = _player.transform.position - transform.position;
        _direction.Normalize();

        if (_direction.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }
    }

    protected override void Move()
    {
        transform.Translate(_direction * _moveSpeed * Time.deltaTime, Space.World);
    }
}
