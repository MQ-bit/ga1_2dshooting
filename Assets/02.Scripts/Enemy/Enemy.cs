using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private int _health = 10;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected int _damage;
    [SerializeField] private GameObject _deathEffectPrefab;
    private bool _isDead;

    private void Update()
    {
        if (_isDead || GameSession.InputBlocked) return;
        Move();
    }

    protected abstract void Move();

    protected virtual void OnHit() { }


    public void TakeDamage(int damage)
    {
        // Destroy는 프레임 끝에 처리되므로 중복 처치와 드롭을 막는다.
        if (_isDead || damage <= 0) return;

        _health -= damage;
        OnHit();
        if (_health <= 0)
        {

            _isDead = true;
            SpawnDeathEffect();
            CombatFeedback.Explosion(transform.position);
            if (GameSession.Instance != null) GameSession.Instance.RegisterKill();
            ItemDropper itemDropper = GetComponent<ItemDropper>();
            if (itemDropper != null)
            {
                itemDropper.TryDrop();
            }

            Destroy(gameObject);
        }

 
    }
    private void SpawnDeathEffect()
    {
        if (_deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale *= 0.4f;
            Destroy(effect, 6f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;
        if (GameSession.InputBlocked) return;
        Player player = other.GetComponentInParent<Player>();
        if (player == null) return;

        // 총알처럼 대상과 충돌한 자신을 먼저 삭제한다.
        _isDead = true;
        SpawnDeathEffect();
        Destroy(gameObject);

        player.TakeDamage(_damage);
    }
}
