using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    private static readonly int HorizontalInput = Animator.StringToHash("x");

    [Header("Movement")]
    [FormerlySerializedAs("Speed")]
    [SerializeField, Min(0f)] private float _speed = 5f;
    [SerializeField, Min(0f)] private float _maxMoveSpeed = 8f;
    [SerializeField, Min(0f)] private float _debugSpeedStep = 1f;

    [Header("Movement Bounds")]
    [FormerlySerializedAs("MaxPositionY")]
    [SerializeField] private float _maximumY = 4.5f;
    [FormerlySerializedAs("MinPositionY")]
    [SerializeField] private float _minimumY = -4.5f;
    [FormerlySerializedAs("MaxPositionX")]
    [SerializeField] private float _maximumX = 8f;
    [FormerlySerializedAs("MinPositionX")]
    [SerializeField] private float _minimumX = -8f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        HandleDebugSpeedInput();
    }

    public void IncreaseMoveSpeed(float amount)
    {
        if (amount <= 0f) return;

        _speed = Mathf.Max(_speed, Mathf.Min(_speed + amount, _maxMoveSpeed));
    }

    private void HandleDebugSpeedInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _speed = Mathf.Min(_speed + _debugSpeedStep, _maxMoveSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            _speed = Mathf.Max(0f, _speed - _debugSpeedStep);
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _animator.SetInteger(HorizontalInput, (int)horizontal);

        Vector2 direction = new Vector2(horizontal, vertical).normalized;
        Vector2 newPosition = transform.position + (Vector3)direction * (_speed * Time.deltaTime);
        newPosition.y = Mathf.Clamp(newPosition.y, _minimumY, _maximumY);

        if (newPosition.x > _maximumX)
        {
            newPosition.x = _minimumX;
        }
        else if (newPosition.x < _minimumX)
        {
            newPosition.x = _maximumX;
        }

        transform.position = newPosition;
    }
}
