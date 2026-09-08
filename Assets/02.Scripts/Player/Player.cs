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

        return true;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        
      
        
        if (_health <= 0)
        {   
            SpawnDeathEffect();
            Destroy(gameObject);
        }
    }
    private void SpawnDeathEffect()
    {
        Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
    }
}
