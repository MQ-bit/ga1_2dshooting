using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(Animator))]
public class Bomb : MonoBehaviour
{
    public float MoveSpeed = 1f;
    private const float LifeTime = 3f;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private float _elapsedTime;
    private static readonly int TimeParameter = Animator.StringToHash("Time");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.gravityScale = 0f;
        _rigidbody.useFullKinematicContacts = true;
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_animator == null) return;
        _animator.SetFloat("tick", _elapsedTime);
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + Vector2.up * (MoveSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Resolve child colliders through their physics body without searching parents.
        Rigidbody2D body = other.attachedRigidbody;
        GameObject target = body != null ? body.gameObject : other.gameObject;
        if (!target.CompareTag("Enemy")) return;

        if (target.TryGetComponent<Enemy>(out var enemy))
        {
            // Keep the enemy's death effects and item drops.
            enemy.TakeDamage(int.MaxValue);
        }
    }
}
