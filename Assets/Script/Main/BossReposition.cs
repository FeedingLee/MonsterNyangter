using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BossReposition : MonoBehaviour
{
    [Header("#BossReposion")]
    public float fallingWaitTime;      // 낙하하기 전 대기 시간
    public float fallingDistance;       // 이 이상 플레이어와 멀어지면 낙하 시작
    public float fallingSpeed;          // 낙하 속도
    public Transform targetTransform;   // 플레이어 트랜스폼
    public bool isBossMove;             // 보스 이동 관련 bool 변수
    public bool isBossFalling;          // 보스 순간이동 관련 bool 변수 
    public float WaitReposition;        // 보스 Reposition 쿨타임
    public BossPattern bossPattern;
    float time;
    Transform bossTransform;            // 자신(보스) 트랜스폼
    Rigidbody2D bossRigid;              // 자신(보스) Rigidbody2D
    Vector2 dirVec;                     // 이동 방향
    GameObject bossShadow;              // 그림자를 저장할 오브젝트
    Animator anim;                      // 보스 애니메이터 

    void Awake()
    {
        bossTransform = transform;
        bossRigid = GetComponent<Rigidbody2D>();
        isBossMove = true;
        isBossFalling = false;
        dirVec = Vector2.zero;
        time = 5;
        targetTransform = GameManager.instance.player.transform;
        bossPattern = gameObject.GetComponent<BossPattern>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Reposition 쿨타임 계산
        time += Time.deltaTime;
        if (time < WaitReposition)
        {
            //Debug.Log("time: " + time);
            return;
        }

        // 보스가 공격중일때는 Reposition을 막음
        if (bossPattern.isBossAttacking || !bossPattern.isBossLive)
        {
            return;
        }

        // 플레이어와 보스 사이의 거리 측정
        float distance = Vector3.Distance(targetTransform.position, bossTransform.position);
        
        // 거리가 지정한 값보다 멀어졌을 경우 & BossReposion이 진행중이지 않을 경우
        // & 보스의 공격이 진행중이지 않을 경우
        if (distance > fallingDistance && 
            !isBossFalling &&
            !bossPattern.isBossAttacking)
        {
            Debug.Log("isBossAttacking: " + bossPattern.isBossAttacking + " in " + this);
            // Reposition 시작
            StartCoroutine("StartBossReposition");
        }

        // 보스의 목표 방향이 존재할 경우
        if (dirVec != Vector2.zero)
        {
            Vector2 nextVec = dirVec.normalized * fallingSpeed * Time.fixedDeltaTime;
            bossRigid.MovePosition(bossRigid.position + nextVec);
        }
    }

    IEnumerator StartBossReposition()
    {
        // 보스 공격 패턴 중지
        Debug.Log("Stop Coroutine Boss Pattern in " + this);
        StopCoroutine(bossPattern.repeatActionCoroutine);

        // 보스 이동 제한
        isBossMove = false;
        isBossFalling = true;

        // Player와 Boss 의 방향벡터 계산
        Vector3 direction = (targetTransform.position - bossTransform.position).normalized;

        // BossReposion
        bossTransform.position = targetTransform.position + direction * 2.0f;

        // 위치에 그림자 생성
        bossShadow = GameManager.instance.pool.GetEnemy(3);
        bossShadow.transform.position = bossTransform.position - new Vector3(0,1,0);

        // 그림자 추적 허용
        bossShadow.GetComponent<BossShadow>().IsTraceTarget = true;

        // Boss 위치 하늘로 설정
        bossTransform.position += new Vector3(0, 500, 0);

        // 하늘에서 떨어지는 시간 대기
        yield return new WaitForSeconds(fallingWaitTime - 0.5f);

        // 그림자 추적 종료
        bossShadow.GetComponent<BossShadow>().IsTraceTarget = false;

        // 그림저 추적 종료 후 0.5초 뒤 하강 시작
        yield return new WaitForSeconds(0.5f);

        // Boss 위치 그림자 위로 설정
        bossTransform.position = bossShadow.transform.position + new Vector3(0, 25, 0);

        // 하강 목표 방향 설정
        dirVec = bossShadow.GetComponent<Transform>().position - bossTransform.position;

        yield return new WaitForFixedUpdate();
    }

    IEnumerator WaitAnim(float waitTime)
    {
        // 착지 애니메이션이 재생중인 동안 대기
        yield return new WaitForSeconds(waitTime);
        Debug.Log("wiatTime : " + waitTime);

        // 보스 이동 제한 해제
        isBossMove = true;
        isBossFalling = false;

        //보스 패턴 재시작
        Debug.Log("Boss Pattern ReStart");
        bossPattern.repeatActionCoroutine =
            StartCoroutine(bossPattern.RepeatAction());

        time = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BossShadow")
        {
            // 착지 애니메이션 재생
            anim.SetTrigger("isBossRanding");

            // 하강 목표 방향 초기화
            dirVec = Vector2.zero;

            // 그림자와 충돌시 보스 하강 멈춤
            bossRigid.velocity = Vector3.zero;

            // 그림자 비활성화
            bossShadow.SetActive(false);

            StartCoroutine(WaitAnim(0.5f));
        }
    }
}
