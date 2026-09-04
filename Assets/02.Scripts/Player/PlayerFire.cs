using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // 목표: 스페이스바를 누를 때마다 총알을 생성해서 발사하고 싶다.
    // 필요 속성
    // - 총알 프리팹
    public GameObject BulletPrefab;

    // - 생성 위치(총구)
    public Transform LeftFirePoint;
    public Transform RightFirePoint;

    // - 쿨타이머
    public float CoolTime = 0.5f;
    public float CoolTimer = 0;

    // - 오토 모드
    public bool AutoFireMode = false;

    private void Start()
    {
        CoolTimer = CoolTime;
    }


    private void Update()
    {
        // 오토 공격 모드 토글
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AutoFireMode = !AutoFireMode;
        }

        // 0. 쿨타이머 감소
        CoolTimer -= Time.deltaTime;

        // 1. 쿨타이머가 0초 이하이고 && (스페이스바를 누르거나 || 오토 모드라면)
        if (CoolTimer <= 0 && (Input.GetKeyDown(KeyCode.Space) || AutoFireMode))
        {
            // 2. 발사
            Fire();

            // 3. 쿨타이머 초기화
            CoolTimer = CoolTime;
        }
    }

    private void Fire()
    {
        // 2. 총알 프리팹을 생성한다.
        // Instantiate는 프리팹을 복사해서 (Monobehaviour를 상속받는)게임 오브젝트를 생성하고 씬에 넣어주는 기능
        GameObject leftBullet = Instantiate(BulletPrefab);
        leftBullet.transform.position = LeftFirePoint.position; // 생성한 총알의 위치를 총구의 위치로

        GameObject rightBullet = Instantiate(BulletPrefab);
        rightBullet.transform.position = RightFirePoint.position; // 생성한 총알의 위치를 총구의 위치로
    }
}