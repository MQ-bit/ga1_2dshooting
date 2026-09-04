using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 목적: 총알을 위로 움직이고 싶다.
    public float MoveSpeed;
    public int Damage;


    private void Update()
    {
        Vector2 direction = Vector2.up; //  new Vector2(0, 1);
        transform.Translate(direction * MoveSpeed * Time.deltaTime);
    }

    // 트리거 관련 이벤트
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 나죽고!
        Destroy(this.gameObject);

        // 충돌한 친구가 Enemy일때만 죽여쁠자!
        if (other.gameObject.CompareTag("Enemy"))
        {
            // GetComponent<타입>() -> 게임 오브젝트가 가지고 있는 컴포넌트를 참조
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            // 응집도는 높히고, 결합도는 낮춰라
            // 결합도란 묻는거.. 매번 묻는거..
            // 무적모드 검사하고
            // 방어력 검사.. 
            if(enemy!=null)
            enemy.TakeDamage(Damage);
        }
    }


    // 충돌 관련 이벤트 (Enter -> Stay -> Exit)

    // 충돌이 시작되면 호출되는 이벤트 함수
   

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Debug.Log("충돌중이다.!");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Debug.Log("충돌이 끝났다.!");
    }
}