using UnityEngine;

public class DownwardEnemy : Enemy
{
    protected override void Move()
    {
        Vector2 direction = Vector2.down;

        transform.Translate(direction * _moveSpeed * Time.deltaTime);
    }
}