using UnityEngine;

public class GeminiReviewPractice : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;

    private void FixedUpdate()
    {
        if (playerRigidbody == null)
        {
            return;
        }

        playerRigidbody.AddForce(Vector3.forward * 10f);
    }
}