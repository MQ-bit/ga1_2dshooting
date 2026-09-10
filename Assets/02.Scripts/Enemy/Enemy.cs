using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private int _health = 10;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected int _damage;
    [SerializeField] private GameObject _deathEffectPrefab;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField, Range(0f, 1f)] private float _hitSoundVolume = 0.5f;

    private AudioSource _audioSource;
    private bool _isDead;

    public int Health => _health;

    protected virtual void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Move();
    }

    protected abstract void Move();

    protected virtual void OnHit()
    {
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0) return;

        _health -= damage;
        if (_hitSound != null) _audioSource.PlayOneShot(_hitSound, _hitSoundVolume);
        OnHit();

        if (_health > 0) return;

        _health = 0;
        _isDead = true;
        SpawnDeathEffect();
        ScoreManger scoreManger = GameObject.FindAnyObjectByType<ScoreManger>();
        scoreManger.AddScore(100);
        
        if (TryGetComponent(out ItemDropper itemDropper))
        {
            itemDropper.TryDrop();
        }

        PrepareForRemoval();
        float delay = _hitSound != null ? _hitSound.length : 0f;
        Destroy(gameObject, delay);
    }

    private void SpawnDeathEffect()
    {
        if (_deathEffectPrefab != null)
        {
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void PrepareForRemoval()
    {
        foreach (Collider2D enemyCollider in GetComponentsInChildren<Collider2D>())
        {
            enemyCollider.enabled = false;
        }

        foreach (Renderer enemyRenderer in GetComponentsInChildren<Renderer>())
        {
            enemyRenderer.enabled = false;
        }

        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead || !other.CompareTag("Player")) return;

        _isDead = true;
        if (other.TryGetComponent(out Player player))
        {
            player.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}
