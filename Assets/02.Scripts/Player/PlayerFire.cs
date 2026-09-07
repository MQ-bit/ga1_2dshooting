using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Header("Main Bullets")]
    public GameObject BulletPrefab;
    public Transform LeftFirePoint;
    public Transform RightFirePoint;

    [Header("Auxiliary Bullets")]
    [SerializeField] private float _auxiliaryBulletSpeed = 7f;
    [SerializeField] private int _auxiliaryBulletDamage = 2;
    [SerializeField] private Vector3 _auxiliaryBulletScale = new Vector3(0.15f, 0.45f, 1f);
    [SerializeField] private Color _auxiliaryBulletColor = Color.cyan;
    [SerializeField] private float _auxiliaryBulletOffset = 0.35f;

    [Header("Fire Settings")]
    public float CoolTime = 0.5f;
    public float CoolTimer;
    public bool AutoFireMode;
    [SerializeField, Min(0.01f)] private float _minCoolTime = 0.1f;

    public void IncreaseAttackSpeed(float increaseRatio)
    {
        if (increaseRatio <= 0f || CoolTime <= 0f) return;

        // 공격 속도 20% 증가 = 기존 발사 간격 / 1.2.
        float previousCoolTime = CoolTime;
        CoolTime = Mathf.Min(previousCoolTime,
            Mathf.Max(Mathf.Max(0.01f, _minCoolTime), previousCoolTime / (1f + increaseRatio)));
        if (CoolTimer > 0f)
        {
            CoolTimer *= CoolTime / previousCoolTime;
        }
    }

    private void Start()
    {
        CoolTimer = CoolTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AutoFireMode = !AutoFireMode;
        }

        CoolTimer -= Time.deltaTime;

        if (CoolTimer <= 0f && (Input.GetKeyDown(KeyCode.Space) || AutoFireMode))
        {
            Fire();
            CoolTimer = CoolTime;
        }
    }

    private void Fire()
    {
        CreateBullet(LeftFirePoint.position);
        CreateBullet(RightFirePoint.position);

        // 좌우에 속도와 모양이 다른 보조 총알을 한 발씩 발사한다.
        CreateAuxiliaryBullet(LeftFirePoint, Vector2.left);
        CreateAuxiliaryBullet(RightFirePoint, Vector2.right);
    }

    private void CreateBullet(Vector3 position)
    {
        GameObject bullet = Instantiate(BulletPrefab);
        bullet.transform.position = position;
    }

    private void CreateAuxiliaryBullet(Transform firePoint, Vector2 offsetDirection)
    {
        GameObject auxiliaryBullet = Instantiate(BulletPrefab);
        auxiliaryBullet.transform.position =
            firePoint.position + (Vector3)(offsetDirection * _auxiliaryBulletOffset);
        auxiliaryBullet.transform.localScale = _auxiliaryBulletScale;

        Bullet bullet = auxiliaryBullet.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.MoveSpeed = _auxiliaryBulletSpeed;
            bullet.Damage = _auxiliaryBulletDamage;
        }

        SpriteRenderer spriteRenderer = auxiliaryBullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = _auxiliaryBulletColor;
        }
    }
}
