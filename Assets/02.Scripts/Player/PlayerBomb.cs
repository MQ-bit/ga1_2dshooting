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
        if (GameSession.InputBlocked) return;
        CoolTimer = Mathf.Max(0f, CoolTimer - Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.B) && CoolTimer <= 0f)
        {
            if (BombPrefab == null) return;

            Vector3 position = LeftFirePoint != null && RightFirePoint != null
                ? (LeftFirePoint.position + RightFirePoint.position) * 0.5f
                : transform.position;
            Instantiate(BombPrefab, position, Quaternion.identity);
            CoolTimer = MinCoolTime;
            if (GameSession.Instance != null) GameSession.Instance.Announce("PULSE BOMB", "EXPANDING SHOCKWAVE", 1.3f);
        }
    }
}
