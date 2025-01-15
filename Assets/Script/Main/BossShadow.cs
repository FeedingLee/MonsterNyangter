using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShadow : MonoBehaviour
{
    public bool IsTraceTarget;          // 플레이어 추적 허용
    public Rigidbody2D targetRigid;     // 플레이어 Rigidbody2D
    public float speed;                 // 그림자 이동 속도
    Rigidbody2D rig;                    // 그림자 Rigidbody2D

    private void Awake()
    {
        speed = GameManager.instance.player.speed - 1.0f;
        IsTraceTarget = false;
        rig = GetComponent<Rigidbody2D>();
        targetRigid = GameManager.instance.player.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // 플레이어 추적 허용 상태일 때
        if (IsTraceTarget)
        {
            // 그림자 이동 로직
            Vector2 dirVec = targetRigid.position - rig.position;
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rig.MovePosition(rig.position + nextVec);
        }
    }
}
