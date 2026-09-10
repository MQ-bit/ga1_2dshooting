using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 10;
    [SerializeField, Min(1)] private int _maxHealth = 10;

    [Header("Item Effects")]
    [SerializeField, Min(0f)] private float _attackSpeedIncrease = 0.2f;
    [SerializeField, Min(0)] private int _healthRecovery = 3;
    [SerializeField, Min(0f)] private float _moveSpeedIncrease = 0.5f;

    [Header("Effects")]
    [SerializeField] private GameObject _deathEffectPrefab;
    [SerializeField] private AudioClip _itemSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _deathSound;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public bool TryApplyItem(Item.ItemType type)
    {
        if (_health <= 0) return false;

        switch (type)
        {
            case Item.ItemType.AttackSpeedUp:
                if (!TryGetComponent(out PlayerFire playerFire)) return false;
                playerFire.IncreaseAttackSpeed(_attackSpeedIncrease);
                break;

            case Item.ItemType.HealthRecovery:
                _health = Mathf.Min(_health + _healthRecovery, _maxHealth);
                break;

            case Item.ItemType.MoveSpeedUp:
                if (!TryGetComponent(out PlayerMove playerMove)) return false;
                playerMove.IncreaseMoveSpeed(_moveSpeedIncrease);
                break;

            default:
                return false;
        }

        PlaySound(_itemSound);
        return true;
    }

    public void TakeDamage(int damage)
    {
        if (_health <= 0 || damage <= 0) return;

        _health -= damage;
        if (_health > 0)
        {
            PlaySound(_hitSound);
            return;
        }

        _health = 0;
        PlaySound(_deathSound);
        SpawnDeathEffect();
        PrepareForRemoval();

        float delay = _deathSound != null ? _deathSound.length : 0f;
        Destroy(gameObject, delay);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null) _audioSource.PlayOneShot(clip);
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
        foreach (Collider2D playerCollider in GetComponentsInChildren<Collider2D>())
        {
            playerCollider.enabled = false;
        }

        foreach (Renderer playerRenderer in GetComponentsInChildren<Renderer>())
        {
            playerRenderer.enabled = false;
        }

        foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
        {
            if (behaviour != this) behaviour.enabled = false;
        }
    }
}
