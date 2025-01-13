using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TreeEditor;
using Unity.VisualScripting;
/*using UnityEditor.ShaderGraph.Internal;*/
using UnityEngine;

public class BossPattern : MonoBehaviour
{
    public Rigidbody2D targetRigid;          // 플레이어 추적
    public Spawner spawner;                  // 보스스폰을 위해 선언

    [Header("# BossBullet")]
    public float interval;                   // 발사 반복 간격
    public int bulletCount;                  // 발사 갯수
    public GameObject bullet;                // 발사 오브젝트
    public float BossBulletDamage;           // 발사 데미지
    public float fireSpeed;                  // 발사 속도
    //public int count;                      // 발사 ??? : 필요없는것같아서 일단 주석처리
    Rigidbody2D BossRigid;                   // 발사 오브젝트(Boss) 의 Rigidbody2D
    Animator anim;
    SpriteRenderer spriter;
    Collider2D coll;

    [Header("# BossDash")]
    public float DashSpeed;                  // 돌진 속도
    public bool IsBossDashing;               // 보스 돌진 상태 체크
    public float BossDashDamage;             // 보스 돌진 데미지
    public float BossDashDelay;              // 보스 돌진 대기 시간

    [Header("# BossState")]
    public float BossSpeed;                  // 보스 이동 속도
    public float currentHp;                  // 보스 현재 체력
    public float maxHp;                      // 보스 최대 체력
    public bool isBossLive;                         // 보스 생존 확인
    bool isBossTired;                        // 보스 지침 상태 확인
    public bool isBossAttacking;             // 보스 공격 상태 확인

    public Coroutine repeatActionCoroutine; // 실행 중인 코루틴을 저장할 변수

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        BossRigid = GetComponent<Rigidbody2D>();
        targetRigid = GameManager.instance.player.GetComponent<Rigidbody2D>();
        Debug.Log("targetRigid: " + targetRigid);
    }

    void OnEnable()
    {
        IsBossDashing = false;
        currentHp = maxHp;
        isBossLive = true;
        isBossTired = false;
        isBossAttacking = false;
        coll.enabled = true;
        BossRigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        anim.SetBool("isBossDash", false);
        anim.SetBool("isBossDashReady", false);
        anim.SetBool("isBossTired", false);
    }

    public void Init(BossSpawnData data)
    {
        //anim.runtimeAnimatorController = animCon[data.spriteType];
        BossSpeed = data.speed;
        maxHp = data.health;
        currentHp = data.health;
    }

    private void Start()
    {
        // 오브젝트가 활성화 된 후 코루틴 시작
        // 1220: 보스 패턴 진행중에 텔포 막기 & 텔포중 보스 패턴 막기
        // 코루틴 시작 시 반환값을 저장
        repeatActionCoroutine = StartCoroutine(RepeatAction());
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // 보스가 돌진 패턴시 or 보스 사망시 이동 막음
        if (IsBossDashing || !isBossLive ||
            !transform.GetComponent<BossReposition>().isBossMove)
            return;

        // 보스 이동 로직
        Vector2 dirVec = targetRigid.position - BossRigid.position;
        Vector2 nextVec = dirVec.normalized * BossSpeed * Time.fixedDeltaTime;
        BossRigid.MovePosition(BossRigid.position + nextVec);
    }

    public IEnumerator RepeatAction()
    {
        Debug.Log("BossPattern Start in " + this);

        while (isBossLive)
        {
            // 소환되자마자 발사하는것을 막음, 보스패턴 대기시간 / 2 초 대기
            yield return new WaitForSeconds(interval);

            // 패턴을 랜덤으로 선택
            int number = Random.Range(0, 3);

            //테스트 용도
            //number = 1;

            if (gameObject.GetComponent<BossReposition>().isBossFalling)
            {
                // 보스가 텔포중일 때 공격기능 막음
                //Debug.Log("isBossFalling, stop attack in " + this);
                StopCoroutine(repeatActionCoroutine);
            }

            Debug.Log("BossPattern: " + number);            
            // 반복할 동작
            switch (number)
            {
                case 0:
                    isBossAttacking = true;
                    // 사방으로 발사
                    Fire();
                    isBossAttacking = false;
                    break; 
                case 1:
                    isBossAttacking = true;
                    // 일직선으로 연속 발사
                    for (int i = 0; i < bulletCount; i++)
                    {
                        // 0.1초(밸런스에 따라 조절) 안에 일정 간격으로 발사횟수만큼 발사
                        yield return new WaitForSeconds(1.0f / bulletCount);
                        Fires();
                    }
                    isBossAttacking = false;
                    break;
                case 2:
                    // 돌진 대기 n초
                    IsBossDashing = true;
                    isBossAttacking = true;

                    // 보스 돌진 애니메이션 재생
                    anim.SetBool("isBossDashReady", true);
                    // BossDashDelay 만큼 시간이 지난후 Setbool false

                    anim.speed = 0.15f;
                    Debug.Log("animator speed : " + anim.speed);

                    yield return new WaitForSeconds(2f);

                    anim.speed = 1.0f;
                    Debug.Log("animator speed : " + anim.speed);

                    Debug.Log("BossDash in Pattern up BossDash()");
                    BossDash(); 
                    
                    // 2초동안 플레이어와 충돌하지 않으면 멈춤
                    yield return new WaitForSeconds(2f);

                    // 보스가 2초가 지나기 전에 플레이어와 충돌하면 아래 코드를 스킵
                    if (IsBossDashing)
                    {
                        BossStop();
                    }

                    break;
            }

            // 보스패턴 대기시간 / 2초 대기
            //yield return new WaitForSeconds(interval / 2f);
            Debug.Log("BossPaettern End in " + this);
        }
    }

    // 보스 사망 모션 후 1초동안 대기, 이후 보스 오브젝트 비활성화
    IEnumerator BossDead1sec()
    {
        Debug.Log("BossDeadCorutine");
        yield return new WaitForSeconds(1.0f);        
        Dead();
    }

    // 발사체 생성 함수
    void Fire()
    {
        anim.SetTrigger("BossFire");

        // 사방으로 발사하는 패턴
        float angleStep = 360f / bulletCount;  // 각 발사체 사이의 각도 차이

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;  // 각 발사체의 각도
            float rad = angle * Mathf.Deg2Rad;  // 각도를 라디안으로 변환

            // 방향 벡터 계산
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

            // 발사체 생성 및 초기화          
            Transform bullet = GameManager.instance.pool.GetEnemy(2).transform;
            bullet.position = transform.position/* + new Vector3(0, 1, 0)*/;  // 총알의 시작 위치
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 회전 설정
            bullet.GetComponent<BossBullet>().Init(BossBulletDamage, dir * fireSpeed);   // 총알 초기화
        }
    }

    void Fires()
    {
        anim.SetTrigger("BossFire");

        // 발사체 발사 방향 계산
        Vector2 dirVec = targetRigid.position - BossRigid.position;
        Vector2 nextVec = dirVec.normalized;

        // 발사체 생성 및 초기화
        Transform bullet = GameManager.instance.pool.GetEnemy(2).transform;
        bullet.position = transform.position + new Vector3(0, 1, 0);                // 총알의 시작 위치
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, nextVec);           // 회전 설정
        bullet.GetComponent<BossBullet>().Init(BossBulletDamage, nextVec * fireSpeed * 3f);   // 총알 초기화
        // 연속발사와 사방발사 속도가 동일할 시 연속발사 속도가 너무 느려보여서 임시로 3배 빠르게 설정

    }

    void BossDash()
    {
        // 보스 돌진 애니메이션 재생
        anim.SetBool("isBossDashReady", false);
        anim.SetBool("isBossDash", true);

        // 보스 돌진 패턴
        IsBossDashing = true;

        // 발사체 발사 방향 계산
        Vector2 dirVec = targetRigid.position - BossRigid.position;
        Vector2 nextVec = dirVec.normalized;

        // 돌진
        BossRigid.velocity = nextVec * DashSpeed;
    }

    public void BossStop()
    {        
        // Player 충돌 부분에서 선언하기위해 public으로 설정
        if (IsBossDashing)
        {
            Debug.Log("BossStop");
            BossRigid.velocity = Vector3.zero;
            isBossAttacking = false;
            IsBossDashing = false;
            anim.SetBool("isBossDash", false);
            anim.SetTrigger("isBossDashDone");
        }
    }

    void Dead()
    {
        // 보스 사망 시 비활성화
        Debug.Log("BossDeadFuntion!");
        gameObject.SetActive(false);

        // 승리 이벤트 시작
        GameManager.instance.GameVictory();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        // 접촉한 오브젝트가 bullet이 아니면 리턴
        if (!collision.CompareTag("Bullet"))
            return;
        //Debug.Log("TriggerEnterCollisionName: " + collision.name);

        currentHp -= collision.GetComponent<Bullet>().damage;
        /* 
         * 넉백 구현 부분
        */

        if (currentHp > 0)
        {
            // 체력이 남은 경우 피격 애니메이션, 사운드 재생
            // 보스는 피격 애니메이션 X
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);

            if (!isBossTired && currentHp <= maxHp / 2)
            {
                Debug.Log("BossHp is half");

                // 보스 지침상태로 전환
                isBossTired = true;
                anim.SetBool("isBossTired", true);

                // 이동속도 반으로 감소
                BossSpeed /= BossSpeed;
            }
        } 
        else
        {
            isBossLive = false;
            Debug.Log("BossDead!");
            coll.enabled = false;
            BossRigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            isBossTired = false;
            StartCoroutine(BossDead1sec());
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
            //spawner.bossSpawn = false;
            //if (GameManager.instance.isLive)
            //    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }
}
