using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    public GameObject BombPrefab;
    public Transform LeftFirePoint;
    public Transform RightFirePoint;

    private const float MinCoolTime = 10f;
    public float FireRate => MinCoolTime;
    public float CoolTimer = 0f;

    private void Update()
    {
        CoolTimer = Mathf.Max(0f, CoolTimer - Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.B) && CoolTimer <= 0f)
        {
            if (BombPrefab == null) return;

            Vector3 position = LeftFirePoint != null
                ? LeftFirePoint.position
                : transform.position;
            Instantiate(BombPrefab, position, Quaternion.identity);
            CoolTimer = MinCoolTime;
        }
    }
}
