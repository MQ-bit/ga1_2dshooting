using UnityEngine;

public class HomingEnemy : Enemy
{
    private Animator _animator;

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

    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    protected override void Move()
    {
        if (_player == null)
        {
            return;
        }

        Vector2 direction = _player.transform.position - transform.position;
        direction.Normalize();

        if (direction.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }

        transform.Translate(direction * _moveSpeed * Time.deltaTime, Space.World);
    }
}
