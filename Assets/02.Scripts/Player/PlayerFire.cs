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

        // Convert an attack-speed multiplier into a shorter firing interval.
        float previousCoolTime = CoolTime;
        CoolTime = Mathf.Min(previousCoolTime,
            Mathf.Max(_minCoolTime, previousCoolTime / (1f + increaseRatio)));
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
        
        // Fire one configured auxiliary projectile from each side.
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
            bullet.Configure(_auxiliaryBulletSpeed, _auxiliaryBulletDamage);
        }

        SpriteRenderer spriteRenderer = auxiliaryBullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = _auxiliaryBulletColor;
        }
    }
}
