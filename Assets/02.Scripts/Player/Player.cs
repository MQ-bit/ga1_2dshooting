using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 10;
    [SerializeField, Min(1)] private int _maxHealth = 10;

    [Header("Item Effects")]
    [SerializeField, Min(0f)] private float _attackSpeedIncrease = 0.2f;
    [SerializeField, Min(0)] private int _healthRecovery = 3;
    [SerializeField, Min(0f)] private float _moveSpeedIncrease = 0.5f;
    [SerializeField] private GameObject _deathEffectPrefab;
    [SerializeField, Min(0f)] private float _invulnerabilityTime = 1.2f;
    private float _invulnerableUntil;
    private SpriteRenderer _sprite;
    private Color _originalColor;
    public int Health => _health;
    public int MaxHealth => _maxHealth;

    private void Awake()
    {
        _maxHealth = Mathf.Max(1, _maxHealth);
        _health = Mathf.Clamp(_health, 1, _maxHealth);
        _sprite = GetComponentInChildren<SpriteRenderer>();
        if (_sprite != null) _originalColor = _sprite.color;
    }

    private void LateUpdate()
    {
        if (_sprite == null) return;
        Color color = _originalColor;
        if (Time.time < _invulnerableUntil) color.a *= Mathf.PingPong(Time.time * 12f, 1f) > 0.5f ? 0.3f : 1f;
        _sprite.color = color;
    }
    
    public bool TryApplyItem(Item.ItemType type)
    {
        // 사망 처리가 예약된 플레이어는 아이템을 획득하지 않는다.
        if (_health <= 0) return false;

        switch (type)
        {
            case Item.ItemType.AttackSpeedUp:
                PlayerFire playerFire = GetComponent<PlayerFire>();
                if (playerFire == null) return false;
                playerFire.IncreaseAttackSpeed(_attackSpeedIncrease);
                break;

            case Item.ItemType.HealthRecovery:
                _health += Mathf.Min(_healthRecovery, Mathf.Max(0, _maxHealth - _health));
                break;

            case Item.ItemType.MoveSpeedUp:
                PlayerMove playerMove = GetComponent<PlayerMove>();
                if (playerMove == null) return false;
                playerMove.IncreaseMoveSpeed(_moveSpeedIncrease);
                break;

            default:
                return false;
        }

        CombatFeedback.Pickup(transform.position);
        if (GameSession.Instance != null)
            GameSession.Instance.Announce(type == Item.ItemType.HealthRecovery ? "HULL REPAIRED" :
                type == Item.ItemType.AttackSpeedUp ? "FIRE RATE UP" : "THRUST UP", "UPGRADE ACQUIRED", 1.4f);
        return true;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || _health <= 0 || Time.time < _invulnerableUntil || GameSession.InputBlocked) return;
        _health = Mathf.Max(0, _health - damage);
        _invulnerableUntil = Time.time + _invulnerabilityTime;
        CombatFeedback.PlayerHit(transform.position);
        
      
        
        if (_health <= 0)
        {   
            SpawnDeathEffect();
            if (GameSession.Instance != null) GameSession.Instance.EndRun();
            Destroy(gameObject);
        }
    }
    private void SpawnDeathEffect()
    {
        if (_deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale *= 0.65f;
            Destroy(effect, 6f);
        }
    }
}
