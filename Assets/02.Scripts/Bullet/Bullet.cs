using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [FormerlySerializedAs("MoveSpeed")]
    [SerializeField, Min(0f)] private float _moveSpeed = 5f;
    [FormerlySerializedAs("Damage")]
    [SerializeField, Min(0)] private int _damage = 1;
    [SerializeField] private LayerMask _damageableLayers;

    public void Configure(float moveSpeed, int damage)
    {
        _moveSpeed = Mathf.Max(0f, moveSpeed);
        _damage = Mathf.Max(0, damage);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * (_moveSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D body = other.attachedRigidbody;
        GameObject target = body != null ? body.gameObject : other.gameObject;
        if ((_damageableLayers.value & (1 << target.layer)) == 0) return;
        if (!target.TryGetComponent(out Enemy enemy)) return;

        enemy.TakeDamage(_damage);
        Destroy(gameObject);
    }
}
