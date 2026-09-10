using UnityEngine;

public class HomingEnemy : Enemy
{
    private Animator _animator;
    private AudioSource _damagedAudioSource;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _damagedAudioSource = GetComponent<AudioSource>();
    }

    protected override void OnHit()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Hit");
    }

    // 캐싱: 자주 쓸법한 데이터(객체)를 가까운 곳에 저장해두고 쓰는거
    private GameObject _player;

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
    }

    protected override void Move()
    {
        // 추적 중 플레이어가 사라졌다면 이동 계산을 하지 않는다.
        if (_player == null)
        {
            return;
        }

        // 1. 방향을 구한다.
        Vector2 direction = _player.transform.position - transform.position;
        direction.Normalize();

        // 2. 매 프레임 플레이어를 바라본다. 스프라이트의 정면은 아래쪽이다.
        if (direction.sqrMagnitude > 0f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }

        // 3. 회전과 관계없이 월드 기준 방향으로 이동한다.
        transform.Translate(direction * _moveSpeed * Time.deltaTime, Space.World);
    }
}
