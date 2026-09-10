using System;
using UnityEngine;

public class DestoryZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<Bomb>() != null) return;
        Destroy(other.gameObject);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
