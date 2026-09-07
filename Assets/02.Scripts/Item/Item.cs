using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public enum ItemType { RedCircle, GreenSquare, BlueTriangle }

    [SerializeField] private ItemType _type;
    [SerializeField, Min(0f)] private float _waitTime = 2f;
    [SerializeField, Min(0f)] private float _moveSpeed = 1.5f;

    private Rigidbody2D _rigidbody;
    private GameObject _player;
    private float _timer;
    private bool _collected;

    public ItemType Type => _type;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindWithTag("Player");
    }

    private void FixedUpdate()
    {
        // 대기 중이거나 플레이어가 사라졌다면 정지한다.
        _rigidbody.linearVelocity = Vector2.zero;
        _timer += Time.fixedDeltaTime;
        if (_timer < _waitTime) return;
        if (_player == null) return;

        // 매번 방향을 다시 구해 움직이는 플레이어를 따라간다.
        Vector2 direction = (Vector2)_player.transform.position - _rigidbody.position;
        float distance = Mathf.Min(_moveSpeed * Time.fixedDeltaTime, direction.magnitude);
        _rigidbody.MovePosition(_rigidbody.position + direction.normalized * distance);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponentInParent<Player>();
        if (player == null || !player.TryApplyItem(_type)) return;

        // Destroy가 처리되기 전 다른 콜라이더가 닿아도 한 번만 적용한다.
        _collected = true;
        Destroy(gameObject);
    }
}
