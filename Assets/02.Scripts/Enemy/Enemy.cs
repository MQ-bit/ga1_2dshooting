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
        Move();
    }

    protected abstract void Move();

    protected virtual void OnHit() { }


    public void TakeDamage(int damage)
    {
        // Destroy는 프레임 끝에 처리되므로 중복 처치와 드롭을 막는다.
        if (_isDead) return;

        _health -= damage;
        OnHit();
        if (_health <= 0)
        {

            SpawnDeathEffect();
            
            _isDead = true;
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
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;
        if (!other.CompareTag("Player")) return;

        // 총알처럼 대상과 충돌한 자신을 먼저 삭제한다.
        _isDead = true;
        Destroy(gameObject);

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            // Player 컴포넌트가 없다면 데미지 처리를 하지 않는다.
            return;
        }

      
            
        player.TakeDamage(_damage);
    }
}
