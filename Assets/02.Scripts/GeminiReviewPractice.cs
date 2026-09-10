using UnityEngine;
using UnityEngine.Serialization;

public class GeminiReviewPractice : MonoBehaviour
{
    [FormerlySerializedAs("playerRigidbody")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private float _forwardForce = 10f;

    private void FixedUpdate()
    {
        if (_playerRigidbody == null)
        {
            return;
        }

        _playerRigidbody.AddForce(Vector3.forward * _forwardForce);
    }
}
