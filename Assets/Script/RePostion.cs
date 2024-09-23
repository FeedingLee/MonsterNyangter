using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RePostion : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // AreaTag가 아니면 (조건)
        if (!collision.CompareTag("Area"))
            return;

        // 플레이어의 위치 받아와서 저장하기
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        // Tag 의 Case에 따라 조건 실행
        switch (transform.tag)
        {
            // 무한맵기능은 미사용
            /*
            case "Ground":
                // 플레이어 위치 - 타일맵 위치 계산으로 거리 구해서 방향 구하기
                float diffX = playerPos.x - (myPos.x);
                float diffY = playerPos.y - (myPos.y);
                // True 와 False에 대한 값 넣기
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);
                // 두 오브젝트의 거리 차이에서, X축이 Y축보다 크면 수평 이동
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 70);
                }
                // 두 오브젝트의 거리 차이에서, Y축이 X축보다 크면 수직 이동
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 70);
                }
                // 그 외에는 타일 2개 이동
                else
                {
                    transform.Translate(Vector3.right * dirX * 70);
                    transform.Translate(Vector3.up * dirY * 70);
                }
                break;
            */
            case "Enemy":
                if (coll.enabled)
                {
                    // 만약 그 Enemy가 생존상태라면, 재배치되어 다가옴
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-6, 6), Random.Range(-6, 6), 0);

                    transform.Translate(ran + dist * 2);
                }
                break;
        }
    }
}
