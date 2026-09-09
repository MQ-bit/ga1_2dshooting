using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(Animator))]
public class Bomb : MonoBehaviour
{
    [Min(0f)] public float MoveSpeed = 1f;
    [SerializeField, Min(0.1f)] private float _lifeTime = 3f;
    [SerializeField, Min(0.1f)] private float _startRadius = 0.55f;
    [SerializeField, Min(0.1f)] private float _blastRadius = 2.6f;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private CircleCollider2D _collider;
    private LineRenderer _ring;
    private SpriteRenderer _sprite;
    private float _elapsedTime;
    private int _stage;
    private static readonly int TickParameter = Animator.StringToHash("tick");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.gravityScale = 0f;
        _rigidbody.useFullKinematicContacts = true;
        _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        _collider.isTrigger = true;
        SetRadius(_startRadius);
    }

    private void Start()
    {
        _ring = CombatFeedback.CreateRing(transform, new Color(0.35f, 1f, 0.92f), 0.045f);
        SetRadius(_startRadius);
        CombatFeedback.BombPulse(transform.position, _startRadius);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(_elapsedTime / Mathf.Max(0.1f, _lifeTime));
        // The existing controller switches sprites at tick 1 and tick 2.
        if (_animator.runtimeAnimatorController != null)
            _animator.SetFloat(TickParameter, progress * 3f);
        int stage = Mathf.Min(2, Mathf.FloorToInt(progress * 3f));
        if (stage != _stage)
        {
            _stage = stage;
            CombatFeedback.BombPulse(transform.position, CurrentRadius(progress));
        }
        if (_ring != null)
        {
            CombatFeedback.SetRingRadius(_ring, CurrentRadius(progress));
            Color color = new Color(0.35f, 1f, 0.92f, 1f - progress * 0.65f);
            _ring.startColor = _ring.endColor = color;
        }
        if (progress >= 1f) Destroy(gameObject);
    }

    private float CurrentRadius(float progress)
    {
        return Mathf.Lerp(_startRadius, Mathf.Max(_startRadius, _blastRadius), Mathf.SmoothStep(0f, 1f, progress));
    }

    private void LateUpdate()
    {
        if (_sprite == null || _sprite.sprite == null) return;
        float progress = Mathf.Clamp01(_elapsedTime / Mathf.Max(0.1f, _lifeTime));
        float radius = CurrentRadius(progress);
        float visibleRadius = _sprite.bounds.extents.x;
        if (visibleRadius > 0.001f)
        {
            Vector3 scale = transform.localScale;
            float ratio = radius / visibleRadius;
            transform.localScale = new Vector3(scale.x * ratio, scale.y * ratio, scale.z);
        }
        // Keep enemies readable through the animated core; the ring marks the exact range.
        _sprite.color = new Color(0.65f, 1f, 1f, Mathf.Lerp(0.42f, 0.12f, progress));
        SetRadius(radius);
    }

    private void SetRadius(float radius)
    {
        float scale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        _collider.radius = radius / Mathf.Max(0.001f, scale);
        if (_ring != null) CombatFeedback.SetRingRadius(_ring, radius);
    }

    private void FixedUpdate()
    {
        SetRadius(CurrentRadius(Mathf.Clamp01(_elapsedTime / Mathf.Max(0.1f, _lifeTime))));
        _rigidbody.MovePosition(_rigidbody.position + Vector2.up * (MoveSpeed * Time.fixedDeltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null) enemy.TakeDamage(int.MaxValue);
    }
}
