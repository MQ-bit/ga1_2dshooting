using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public enum ItemType
    {
        AttackSpeedUp = 0,
        HealthRecovery = 1,
        MoveSpeedUp = 2
    }

    [SerializeField] private ItemType _type;
    [SerializeField, Min(0f)] private float _waitTime = 2f;
    [SerializeField, Min(0f)] private float _moveSpeed = 1.5f;
    [SerializeField, Min(0f)] private float _curveHeight = 2f;

    private Rigidbody2D _rigidbody;
    private GameObject _player;
    private float _timer;
    private bool _collected;
    private bool _isFlying;
    private Vector2 _flightStart;
    private Vector2 _controlPoint;
    private float _flightDuration;
    private float _flightProgress;

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
        if (_collected || _moveSpeed <= 0f) return;

        Vector2 target = _player.transform.position;
        if (!_isFlying)
        {
            _isFlying = true;
            _flightStart = _rigidbody.position;
            Vector2 direction = target - _flightStart;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x).normalized;
            float side = Random.value < 0.5f ? -1f : 1f;
            _controlPoint = (_flightStart + target) * 0.5f + perpendicular * (_curveHeight * side);

            // 제어점을 경유하는 거리로 비행 시간을 정한다.
            float pathLength = Vector2.Distance(_flightStart, _controlPoint)
                               + Vector2.Distance(_controlPoint, target);
            _flightDuration = Mathf.Max(pathLength / _moveSpeed, Time.fixedDeltaTime);
        }

        _flightProgress = Mathf.Clamp01(_flightProgress + Time.fixedDeltaTime / _flightDuration);
        float t = _flightProgress;
        float remaining = 1f - t;

        // 2차 베지어 곡선: 도착점은 움직이는 플레이어의 현재 위치로 갱신한다.
        Vector2 position = remaining * remaining * _flightStart
                           + 2f * remaining * t * _controlPoint
                           + t * t * target;
        _rigidbody.MovePosition(position);
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
