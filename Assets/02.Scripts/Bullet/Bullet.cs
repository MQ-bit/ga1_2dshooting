using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Min(0f)] public float MoveSpeed = 8f;
    [Min(1)] public int Damage = 1;
    [SerializeField, Min(0.1f)] private float _lifeTime = 5f;
    private bool _spent;

    private void Start() { Destroy(gameObject, _lifeTime); }

    private void Update()
    {
        transform.Translate(Vector2.up * (MoveSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_spent) return;
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;
        // Only one target can consume this shot in a physics step.
        _spent = true;
        enemy.TakeDamage(Damage);
        CombatFeedback.Hit(transform.position);
        Destroy(gameObject);
    }
}