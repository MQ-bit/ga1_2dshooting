using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    public float Speed = 4f;
    public float MaxPositionY = 3.6f;
    public float MinPositionY = -3.8f;
    public float MaxPositionX = 3.7f;
    public float MinPositionX = -3.7f;
    [SerializeField, Min(0f)] private float _maxMoveSpeed = 8f;
    [SerializeField, Range(0.1f, 1f)] private float _focusSpeedMultiplier = 0.45f;
    [SerializeField] private bool _wrapHorizontally;
    private Animator _animator;
    private Rigidbody2D _body;
    private TrailRenderer _trail;
    private Vector2 _input;
    private static readonly int HorizontalParameter = Animator.StringToHash("x");
    public bool IsFocused { get; private set; }

    public void IncreaseMoveSpeed(float amount)
    {
        if (amount <= 0f) return;
        Speed = Mathf.Max(Speed, Mathf.Min(Speed + amount, _maxMoveSpeed));
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _trail = GetComponent<TrailRenderer>();
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.useFullKinematicContacts = true;
        _body.gravityScale = 0f;
        _body.interpolation = RigidbodyInterpolation2D.Interpolate;
        _body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (GameSession.InputBlocked) { _input = Vector2.zero; return; }
        _input = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
        IsFocused = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (_animator != null) _animator.SetInteger(HorizontalParameter, _input.x == 0f ? 0 : _input.x > 0f ? 1 : -1);
        if (Input.GetKeyDown(KeyCode.E)) Speed = Mathf.Clamp(Speed + 0.5f, 1f, _maxMoveSpeed);
        if (Input.GetKeyDown(KeyCode.Q)) Speed = Mathf.Clamp(Speed - 0.5f, 1f, _maxMoveSpeed);
    }

    private void FixedUpdate()
    {
        if (GameSession.InputBlocked) return;
        float speed = Mathf.Max(0f, Speed) * (IsFocused ? _focusSpeedMultiplier : 1f);
        Vector2 position = _body.position + _input * (speed * Time.fixedDeltaTime);
        position.y = Mathf.Clamp(position.y, MinPositionY, MaxPositionY);
        if (_wrapHorizontally && (position.x > MaxPositionX || position.x < MinPositionX))
        {
            position.x = position.x > MaxPositionX ? MinPositionX : MaxPositionX;
            _body.position = position;
            if (_trail != null) _trail.Clear();
        }
        else
        {
            position.x = Mathf.Clamp(position.x, MinPositionX, MaxPositionX);
            _body.MovePosition(position);
        }
    }
}
