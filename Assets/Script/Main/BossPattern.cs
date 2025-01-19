using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TreeEditor;
using Unity.VisualScripting;
/*using UnityEditor.ShaderGraph.Internal;*/
using UnityEngine;

public class BossPattern : MonoBehaviour
{
    Rigidbody2D BossRigid;                   // 발사 오브젝트(Boss) 의 Rigidbody2D
    Animator anim;
    SpriteRenderer spriter;
    Collider2D coll;

    public Rigidbody2D targetRigid;          // 플레이어 추적
    public Spawner spawner;                  // 보스스폰을 위해 선언

    [Header("# Multi-Directional Firing")]   // 전방향 발사 관련 변수
    public int MBulletCount;                 // 발사 갯수
    public float MBossBulletDamage;          // 발사 데미지
    public float MFireSpeed;                 // 발사 속도

    [Header("# One-Directional Firing")]     // 단일방향 발사 관련 변수
    public int OBulletCount;                 // 발사 갯수
    public float OBossBulletDamage;          // 발사 데미지
    public float OFireSpeed;                 // 발사 속도

/*    [Header("# BossBullet")]
    public int bulletCount;                  // 발사 갯수
    public float BossBulletDamage;           // 발사 데미지
    public float fireSpeed;                  // 발사 속도*/

    [Header("# BossDash")]
    public float DashSpeed;                  // 돌진 속도
    public bool IsBossDashing;               // 보스 돌진 상태 체크
    public float BossDashDamage;             // 보스 돌진 데미지
    public float BossDashDelay;              // 보스 돌진 대기 시간

    [Header("# BossState")]
    public float interval;                   // 다음 패턴까지의 쿨타임
    public float BossSpeed;                  // 보스 이동 속도
    public float currentHp;                  // 보스 현재 체력
    public float maxHp;                      // 보스 최대 체력
    public bool isBossLive;                  // 보스 생존 확인
    bool isBossTired;                        // 보스 지침 상태 확인
    public bool isBossAttacking;             // 보스 공격 상태 확인
    public float MaxDamage;                  // 보스가 한번에 입을 수 있는 최대 피해량

    public Coroutine repeatActionCoroutine;  // 실행 중인 코루틴을 저장할 변수

    void Awake()
    {
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        BossRigid = GetComponent<Rigidbody2D>();
        targetRigid = GameManager.instance.player.GetComponent<Rigidbody2D>();
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

    private void Start()
    {
        // 오브젝트가 활성화 된 후 코루틴 시작
        // 1220: 보스 패턴 진행중에 텔포 막기 & 텔포중 보스 패턴 막기
        // 코루틴 시작 시 반환값을 저장
        //repeatActionCoroutine = StartCoroutine(RepeatAction());
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
        while (isBossLive)
        {
            // 소환되자마자 발사하는것을 막음, 보스패턴 대기시간 / 2 초 대기
            yield return new WaitForSeconds(interval);

            // 패턴을 랜덤으로 선택 @@ 테스트 0~1
            int number = Random.Range(0, 3);
            if (number == 2)
            {
                // 돌진의 빈도는 화염보다 적게
                number = Random.Range(0, 3);
            }

            if (gameObject.GetComponent<BossReposition>().IsBossFalling)
            {
                // 보스가 텔포중일 때 공격기능 막음
                StopCoroutine(repeatActionCoroutine);
            }

            // 테스트
            //number = 2;

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
                    for (int i = 0; i < OBulletCount; i++)
                    {
                        // 0.1초(밸런스에 따라 조절) 안에 일정 간격으로 발사횟수만큼 발사
                        yield return new WaitForSeconds(1.0f / OBulletCount);
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

                    // 보스 돌진대기 포효 사운드 재생
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Anj_DashYelling);

                    // BossDashDelay 만큼 시간이 지난후 Setbool false
                    yield return new WaitForSeconds(BossDashDelay);

                    BossDash();

                    // 2초동안 플레이어와 충돌하지 않으면 멈춤
                    yield return new WaitForSeconds(3.0f);

                    // 보스가 2초가 지나기 전에 플레이어와 충돌하면 아래 코드를 스킵
                    if (IsBossDashing)
                    {
                        BossStop();
                    }

                    break;
            }

            // 보스패턴 대기시간 / 2초 대기
            //yield return new WaitForSeconds(interval / 2f);
        }
    }

    public void StartAttack()
    {
        if (repeatActionCoroutine == null) // 이미 실행 중인 경우 중복 실행 방지
        {
            repeatActionCoroutine = StartCoroutine(RepeatAction());
        }
    }

    public void StopAttack()
    {
        if (repeatActionCoroutine != null) // 실행 중인 코루틴이 있는 경우에만 종료
        {
            StopCoroutine(repeatActionCoroutine);
            repeatActionCoroutine = null;
        }
    }

    // 보스 사망 모션 후 n초동안 대기, 이후 보스 오브젝트 비활성화
    IEnumerator BossDeadsec(float time)
    {
        yield return new WaitForSeconds(time);
        Dead();
    }

    IEnumerator WaitHitChange(float time)
    {
        yield return new WaitForSeconds(time);
        spriter.color = Color.white;
    }

    // 발사체 생성 함수
    public void Fire()
    {
        // 보스 화염 사운드 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Anj_FireShoot);

        anim.SetTrigger("BossFire");

        // 사방으로 발사하는 패턴
        float angleStep = 360f / MBulletCount;  // 각 발사체 사이의 각도 차이

        for (int i = 0; i < MBulletCount; i++)
        {
            float angle = i * angleStep;  // 각 발사체의 각도
            float rad = angle * Mathf.Deg2Rad;  // 각도를 라디안으로 변환

            // 방향 벡터 계산
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

            // 발사체 생성 및 초기화          
            SpawnFireActor(MBossBulletDamage, 3, new Vector3(0, -2.5f, 0), dir, MFireSpeed, true);
        }
    }

    public void Fires()
    {
        // 보스 칼날 사운드 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Anj_Blade);

        anim.SetTrigger("BossFire");

        // 발사체 발사 방향 계산
        Vector2 dir = (targetRigid.position - BossRigid.position).normalized;

        // 발사체 생성 및 초기화
        SpawnFireActor(OBossBulletDamage, 2, new Vector3(0, -2.5f, 0), dir, OFireSpeed, false);
    }

    public void SpawnFireActor(
        float bulletDamage, int index, Vector3 spawnPosition, Vector3 dir, float addspeed, bool isFlip)
    {
        /* 
         * 매개변수 설명
         * bulletDamage : 탄환 데미지
         * spawnmPosition : 탄환 스폰 위치 조절값
         * dir : 탄환의 발사 방향
         * addSpeed : 탄환 속도 조절값
         * isFlip: 탄환 회전(0도 or 180도) 조절값
         */
        // 발사체 생성 및 초기화          
        Transform bullet = GameManager.instance.pool.GetEnemy(2).transform;
        bullet.position = transform.position + spawnPosition;  // 총알의 시작 위치
        if (!isFlip)
        {
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 회전 설정
        }
        else
        {
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, -dir); // 회전 설정
        }
        bullet.GetComponent<BossBullet>().Init(bulletDamage, dir * addspeed, index);   // 총알 초기화
    }

    void BossDash()
    {
        // 보스 대쉬중 사운드 재생
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Anj_Dash);

        // 돌진 시작시 플레이어 외 다른 오브젝트와의 충돌을 막기위해 isTrigger 활성화
        gameObject.GetComponent<CapsuleCollider2D>().isTrigger = true;

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
            // 돌진이 종료되었을 때 isTrigger 비활성화
            gameObject.GetComponent<CapsuleCollider2D>().isTrigger = false;

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
        //gameObject.SetActive(false);

        // 승리 이벤트 시작
        GameManager.instance.GameVictory();
    }

    void HitColorChange()
    {
        // 피격색상(빨간색)으로 변경
        spriter.color = new Color(1f, 0.54f, 0.54f, 1f);

        // 0.4초 후 원래색상으로 변경
        StartCoroutine(WaitHitChange(0.4f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 접촉한 오브젝트가 bullet일 경우
        if (collision.CompareTag("Bullet"))
        {
            float damage = collision.GetComponent<Bullet>().damage;

            // 플레이어의 무기 데미지가 최대 데미지보다 높을경우 최대 데미지까지만 피해를 입음
            if (damage >= MaxDamage)
            {
                damage = MaxDamage;
            }

            currentHp -= damage;
            HitColorChange();

            if (currentHp > 0)
            {
                if (!isBossTired && currentHp <= maxHp / 2)
                {
                    // 보스 지침상태로 전환
                    isBossTired = true;
                    anim.SetBool("isBossTired", true);

                    // 이동속도 반으로 감소
                    BossSpeed /= BossSpeed;

                    // 광폭화
                    gameObject.GetComponent<BossBerserkMode>().Init();
                }
            }
            else
            {            
                StopAttack();
                isBossLive = false;
                coll.enabled = false;
                BossRigid.simulated = false;
                spriter.sortingOrder = 1;
                isBossTired = false;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Anj_Dead);
                anim.SetBool("isBossDashReady", false);
                anim.SetBool("isBossDash", false);
                anim.SetBool("Dead", true);
                StartCoroutine(BossDeadsec(2.0f));
            }
        }
    }
}
