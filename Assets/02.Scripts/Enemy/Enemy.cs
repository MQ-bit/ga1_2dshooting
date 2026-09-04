using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private int _health = 10;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected int _damage;

    private void Update()
    {
        Move();
    }

    protected abstract void Move();


    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 총알처럼 대상과 충돌한 자신을 먼저 삭제한다.
        Destroy(gameObject);

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogWarning("플레이어가 null입니다.");
            return;
        }

        player.TakeDamage(_damage);
    }
}
