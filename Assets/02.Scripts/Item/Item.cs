using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType _type;
    [SerializeField] private float _value;

    private const float WaitTime = 2f;
    private float _waitTimer;
    private const float MoveSpeed = 5f;

    private Player _player;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        string animationName = _type switch
        {
            ItemType.Heal => "HealthRecovery",
            ItemType.FireRateUp => "AttackSpeedUp",
            _ => _type.ToString()
        };
        int stateHash = Animator.StringToHash($"Base Layer.{animationName}");
        if (_animator.HasState(0, stateHash))
        {
            _animator.Play(stateHash, 0, 0f);
        }

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogWarning("플레이어를 찾을 수 없습니다.");
            return;
        }
    }

    private void Update()
    {
        _waitTimer += Time.deltaTime;
        if (_waitTimer >= WaitTime)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        if (_player == null) return;

        Vector2 direction = _player.transform.position - transform.position;
        direction.Normalize();
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogWarning("플레이어 태그 오브젝트에 플레이어 컴포넌트가 없습니다.");
            return;
        }

        switch (_type)
        {
            case ItemType.Heal:
                player.Heal((int)_value);
                //Debug.Log($"플레이어 체력: {player.Health}");
                break;

            case ItemType.MoveSpeedUp:
                PlayerMove playerMove = player.GetComponent<PlayerMove>();
                playerMove.SpeedUp(_value);
                //Debug.Log($"플레이어 이동속도: {playerMove.Speed}");
                break;

            case ItemType.FireRateUp:
                PlayerFire playerFire = player.GetComponent<PlayerFire>();
                playerFire.FireRateUp(_value);
                //Debug.Log($"플레이어 공격속도: {playerFire.FireRate}");
                break;
        }

        Destroy(gameObject);
    }
}
