using UnityEngine;

public class DownwardEnemy : Enemy
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

    protected override void Move()
    {
        Vector2 direction = Vector2.down;

        transform.Translate(direction * _moveSpeed * Time.deltaTime);
    }
}
