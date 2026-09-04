using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 목적: 키보드 입력에 따라서 플레이어 이동 처리를 하고 싶다.

    // 필요 필드:
    public float Speed;
    public float MaxPositionY;
    public float MinPositionY;
    public float MaxPositionX;
    public float MinPositionX;


    // 매 프레임마다 실행된다.
    // 초당 프레임 실행 횟수는: 별다른 설정이 없을 경우 가능한 많이
    private void Update()
    {
        Move();

        SpeedChange();
    }

    private void SpeedChange()
    {
        // 7. Q/E 버튼 입력을 통한 스피드 업/다운
        if (Input.GetKeyDown(KeyCode.E))
        {
            Speed++;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Speed--;
        }
    }

    private void Move()
    {
        // 1. 키보드 입력을 받는다.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. 키보드 입력에 따라 방향을 구한다.
        Vector2 normalizedDirection = new Vector2(h, v).normalized;

        // 3. 방향과 속력에 따라 이동한다.
        Vector2 newPosition = transform.position + (Vector3)normalizedDirection * Speed * Time.deltaTime;

        // 4. 위치 y에 제한이 있다.
        if (newPosition.y > MaxPositionY)
        {
            newPosition.y = MaxPositionY;
        }
        else if (newPosition.y < MinPositionY)
        {
            newPosition.y = MinPositionY;
        }

        // 5. 양 옆 끝으로 가면 반대쪽 방향으로 이동
        if (newPosition.x > MaxPositionX)
        {
            newPosition.x = MinPositionX;
        }
        else if (newPosition.x < MinPositionX)
        {
            newPosition.x = MaxPositionX;
        }

        transform.position = newPosition;
    }
}