using UnityEngine;

public class Player : MonoBehaviour
{
    // 체력은 외부에서 직접 수정할 수 없고 메서드를 통해서만 변경한다.
    [SerializeField] private int _health = 100;
    public int Health => _health;

    public void TakeDamage(int damage)
    {
        if (damage < 0)
        {
            Debug.LogWarning("대미지는 음수일 수 없습니다.");
            return;
        }

        _health -= damage;
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(int healAmount)
    {
        if (healAmount < 0)
        {
            Debug.LogWarning("힐량은 음수일 수 없습니다.");
            return;
        }

        _health += healAmount;
    }
}
