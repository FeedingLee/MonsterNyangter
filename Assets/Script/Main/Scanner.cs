using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("# EnemyScan")]
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    [Header("# ExpScan")]
    public float expScanRange;          // Exp 구슬 흡수 범위
    public RaycastHit2D[] expTargets;   // Exp 구슬 저장 배열
    public LayerMask expTargetLayer;    // Exp 레이어


    private void FixedUpdate()
    {
        // 원형범위로 모든 객체 검색하기
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();

        // 게임 진행이 멈출 시 경험치 구슬 끌어당김 중단
        if (!GameManager.instance.isLive)
            return;

        // 원형 범위로 Exp 구슬 객체 검색
        if (Item.Super_Magnet == 1)
        {
            // 슈퍼 그물망 활성화 시
            expTargets = Physics2D.CircleCastAll(transform.position, 100, Vector2.zero, 0, expTargetLayer);
            EatExp();
        }
        else
        {
            // 그 외의 경우
            expTargets = Physics2D.CircleCastAll(transform.position, expScanRange, Vector2.zero, 0, expTargetLayer);
            EatExp();
        }
    }

    // 가까운 표적 검색
    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            // 타겟과 플레이어의 거리 계산
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }

    // 범위에 들어온 Exp 구슬들을 플레이어 방향으로 이동시킴
    private void EatExp()
    {
        foreach (RaycastHit2D target in expTargets)
        {
            // 타겟과 플레이어의 거리 계산
            Vector2 myPos = transform.position;
            Vector2 targetPos = target.transform.position;
            Rigidbody2D rigid = target.rigidbody;

            Vector2 dirVec = myPos - targetPos;
            Vector2 nextVec = dirVec.normalized * 5.0f * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);
            rigid.velocity = Vector2.zero;
        }   
    }
}
