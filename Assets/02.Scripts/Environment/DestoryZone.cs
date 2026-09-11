using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    // 나와 충돌한 다른 게임 오브젝트는 누구든 파괴해버리겠다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            other.gameObject.SetActive(false);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
