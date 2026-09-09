using UnityEngine;

public class DestoryZone : MonoBehaviour
{
    private void Awake()
    {
        // Boundary colliders are gameplay helpers, not visible walls.
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>()) renderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Bomb>() != null || other.GetComponentInParent<Player>() != null) return;
        Rigidbody2D body = other.attachedRigidbody;
        Destroy(body != null ? body.gameObject : other.gameObject);
    }
}
