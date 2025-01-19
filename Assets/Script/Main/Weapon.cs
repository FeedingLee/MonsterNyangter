using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [Header("# Weapon Data")]
    public int id;
    public int prefabId;
    public float damage;
    public float speed;             // 무기의 회전 속도
    public float rate;              // 무기의 연사 속도
    public int count;               // 회전하는 무기 갯수

    [Header("# Etc Data")]
    public float memorydamage;      // 데미지 저장
    public float Critical_Damage;   // 크리티컬 데미지
    public float Chargespeed;       // 랜스 돌진 스피드 저장
    Vector3 lastDirection;          // 조이스틱이 마지막으로 향한 방향값

    /* [ 무기 상태 확인 변수 ] */
    bool charm = false;             // [대검] 참 모아베기상태 확인하는 변수
    bool cooldown = false;          // 쿨타임을 확인하는 변수
    bool lancecharge = false;       // [랜스] 돌진상태 확인하는 변수

    /* [ 스크립트 연결 ] */
    float timer;
    Player player;
    ItemData data;
    Item item;
    JoystickController joystickController;
    SpriteRenderer spriteRenderer;

    [Header("# Weapon Level")]
    public static int DB_level;            // 쌍검
    public static int HB_level;            // 활
    public static int SH_level;            // 헤보건
    public static int GS_level;            // 대검
    public static int LC_level;            // 랜스

    void Awake()
    {
        player = GameManager.instance.player;
        joystickController = GameManager.instance.joystickController;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            // 쌍검의 공격 방식
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime * (1 + Gear.stamina_stat));
                if (DB_level == 5)
                {
                    changesprite();
                }
                break;
            // 활의 공격 방식
            case 1:
                timer += Time.deltaTime;
                if (timer > rate)
                {
                    timer = 0f;
                    HuntingBow();
                }
                if (HB_level == 5)
                {
                    changesprite();
                }
                break;
            // 저격용탄 헤비보우건의 공격 방식
            case 2:
                timer += Time.deltaTime;
                if (timer > rate)
                {
                    timer = 0f;
                    Sniper();
                }
                if (SH_level == 5)
                {
                    changesprite();
                }
                break;
            // 대검의 공격 방식 - 회전 중이 아닐 때만 호출
            case 3:
                if (!charm)       // 회전 중이 아닐 때만 실행
                {
                    charm = true; // 회전 중으로 설정
                    GreatSword();
                    StartCoroutine(GreatSwordRotate());
                }
                if (GS_level == 5)
                {
                    changesprite();
                }
                break;
            // 랜스의 공격 방식
            case 4:                
                LanceShieldMod();
                if (!lancecharge)
                {                   
                    lancecharge = true;
                    Lance();
                    StartCoroutine(LanceAttack());
                }

                if (damage == Critical_Damage && player.speed != Chargespeed)       // 돌진모드인데, 속도가 Up되지 않으면
                {
                    player.speed = GameManager.instance.player.memoryspeed + count; // 플레이어의 속도를 count만큼 올린다
                }

                if (LC_level == 5 && damage == Critical_Damage)        
                {
                    spriteRenderer.sprite = data.weaponimage[2];
                }
                else if (LC_level != 5 && damage == Critical_Damage)         
                {
                    spriteRenderer.sprite = data.weaponimage[0];
                }
                break;
        }
    }

    public void changesprite()  
    {
        // 모든 자식 오브젝트의 SpriteRenderer 컴포넌트 가져오기
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            // 각 SpriteRenderer의 스프라이트를 변경
            spriteRenderer.sprite = data.weaponimage[0];
        }
    }

    public void LevelUp(float damage, float speed, float rate, int count)
    {
        this.damage = damage * Character.Damage * (1 + Gear.damage_stat);
        this.speed = speed * Character.WeaponSpeed * (1 + Gear.stamina_stat);
        this.rate = rate * Character.WeaponRate * (1 - Gear.rate_stat);
        this.count = count + Gear.upgrade_stat;

        if (id == 0)
        {
            DualBlades();
        }
        else if (id == 3)
        {
            Critical_Damage = damage * 2 * (1 + Gear.damage_stat);
            memorydamage = Critical_Damage / 2;
            GreatSword();
        }
        else if (id == 4)
        {
            spriteRenderer.sprite = data.weaponimage[1];
            Critical_Damage = damage * 3 * (1 + Gear.damage_stat);
            memorydamage = Critical_Damage / 4;
            Lance();
        }

        switch(id)  // 무기
        {
            case 0: 
                DB_level++;
                break;
            case 1:
                HB_level++;
                break;
            case 2:
                SH_level++;
                break;
            case 3:
                GS_level++;
                break;
            case 4:
                LC_level++;
                break;
        }
    }

    public void Init(ItemData data)
    {
        // Basic Set
        this.data = data;
        if (data.itemId == 0)
        {
            name = "Dual Blades [" + data.itemId + "]";
            DB_level++;
        }
        else if (data.itemId == 1)
        {
            name = "Hunting Bow [" + data.itemId + "]";
            HB_level++;
        }
        else if (data.itemId == 2)
        {
            name = "Sniper HBG [" + data.itemId + "]";
            SH_level++;
        }
        else if (data.itemId == 3)
        {
            name = "GreatSword [" + data.itemId + "]";
            GS_level++;
        }
        else if (data.itemId == 4)
        {
            name = "Lance [" + data.itemId + "]";
            LC_level++;
        }
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // 무기 능력치 셋팅
        id = data.itemId;
        damage = data.baseDamage * Character.Damage * (1 + Gear.damage_stat);
        speed = data.baseSpeed * Character.WeaponSpeed * (1 + Gear.stamina_stat);
        rate = data.baseRate * Character.WeaponRate * (1 - Gear.rate_stat);
        count = (int)(data.baseCount + Gear.upgrade_stat);

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        // 각 근접무기 함수 호출하여 자식 오브젝트 생성
        if (id == 0)
        {
            DualBlades();  // 무기 초기화 시점에서 DualBlades 호출  
        }
        else if (id == 3)
        {
            Critical_Damage = damage * 2;
            memorydamage = Critical_Damage / 2;
            GreatSword();
        }
        else if (id == 4)
        {
            Critical_Damage = damage * 4;
            memorydamage = Critical_Damage / 4;
            Lance();
        }
    }

    // 쌍검 공격, 배치 로직
    void DualBlades()
    {
        for (int index = 0; index < count; index++)                         // 무기 갯수(count) 만큼 루프를 돌림
        {
            Transform bullet;

            if (index < transform.childCount)                               // 이미 존재하는 자식 오브젝트가 있다면 해당 오브젝트를 bullet에 할당
            {
                bullet = transform.GetChild(index);
            }
            else                                                            // 자식 오브젝트가 없다면 새롭게 bullet 생성
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform; // Pool에서 발사체를 가져옴
                bullet.parent = transform;                                  // 새로운 bullet을 현재 Weapon의 자식으로 설정
            }

            bullet.localPosition = Vector3.zero;                            // bullet의 위치를 Weapon의 위치와 동일하게 설정
            bullet.localRotation = Quaternion.identity;                     // bullet의 회전을 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / count;         // bullet을 Weapon 주위로 균일하게 회전 배치
            bullet.Rotate(rotVec);                                          // 회전 적용
            bullet.Translate(bullet.up * 1.5f, Space.World);                // 회전된 방향으로 1.5만큼 이동해 Weapon에서 약간 떨어진 위치로 배치

            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);   // bullet의 데미지 설정 및 방향 초기화
        }
    }

    // 대검 배치 로직
    void GreatSword()
    {
        if (cooldown)                                          // 쿨다운 상태 (참모아 베기가 끝난 뒤) 
        {
            damage = memorydamage;                             // 데미지를 원래 데미지 값으로 변경
            Critical_Damage = damage * 2;
            cooldown = false;                                  // 쿨다운 상태를 해제함
        }
        else if (!cooldown)                                    // 공격 가능 상태 (참모아 베기 중)
        {
            damage = Critical_Damage;                          // 카운트만큼 회전하면, 데미지를 크리티컬 데미지로 변경 ex) count가 3이면 3번째 참모아베기에 데미지 증가     
        }

        for (int i = 0; i < 1; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            bullet.Translate(bullet.up * 3.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }

    // 랜스 배치 로직
    void Lance()
    {
        for (int i = 0; i < 1; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            bullet.Translate(bullet.up * 0f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }

    // 활 배치 + 공격 로직
    void HuntingBow()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Weapon_Range);
    }

    // 헤비보우건 배치 + 공격 로직
    void Sniper()
    {
        Vector3 dir = joystickController.GetInputVector();                              // Joystick 방향 벡터 받아옴                                              
        if (dir == Vector3.zero)                                                        // 정지상태에서는 무작위로 발사
        {
            float randomAngle = Random.Range(0f, 360f);
            dir = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
        }

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Weapon_Sniper);                  // 발사 소리
    }

    // 대검 회전 공격 코루틴
    IEnumerator GreatSwordRotate()
    {
        int rotationCount = 0;                                                          // 회전 횟수를 추적하기 위한 변수

        while (rotationCount < count)                                                   // count 횟수만큼 회전을 반복
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Weapon_GreatSword);          // 무기 사운드
            float rotatedAmount = 0f;                                                   // 회전 누적값을 초기화         

            while (rotatedAmount < (360f * count))                                      // count 수만큼(바퀴) 360도를 회전
            {
                float rotationStep = speed * Time.deltaTime;                            // 매 프레임마다 회전할 양 계산
                transform.Rotate(Vector3.back * rotationStep);                          // 실제 회전 수행
                rotatedAmount += rotationStep;                                          // 누적 회전량 증가
                yield return null;                                                      // 한 프레임 대기
            }
            Vector3 dir = joystickController.GetInputVector();                          // Joystick 방향 벡터 받아옴
            transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);       // 마지막 조이스틱 방향으로, 대검을 위치함

            rotationCount++;                                                            // 한 번의 360도 회전이 끝날 때마다 회전 횟수 증가

            cooldown = true;                                                            // 쿨다운 상태를 true로 만듬 (공격 중지)
            GreatSword();                                                               // 데미지 조정을 위해 GreatSword 함수를 다시 부름
            yield return new WaitForSeconds(rate);                                      // 한 번 회전이 끝난 후 rate만큼 대기
        }
        charm = false;                                                                  // 참모아베기 정지상태로 변경   
    }

    // 랜스 움직이는 로직
    void LanceRotate()
    {
        Vector3 dir = joystickController.GetInputVector();                              // Joystick 방향 벡터 받아옴

        if (dir != Vector3.zero)                                                        // dir이 (0, 0, 0)이 아닐 때만 lastDirection 업데이트
        {
            lastDirection = dir;                                                        // 마지막 방향 업데이트
        }
        else
        {
            dir = lastDirection;                                                        // dir이 (0, 0, 0)이면 마지막 방향 유지
        }
        transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);           // 조이스틱 방향 또는 마지막 방향으로 랜스를 배치함    
    }

    // 랜스 공격 로직
    IEnumerator LanceAttack()
    {
        yield return new WaitForSeconds(rate);                                          // rate 만큼 쿨타임을 기다려야 돌진모드 활성화

        if (lancecharge == true)
        {
            LanceShieldMod();
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Weapon_ChargeMod);           // 무기 사운드
            damage = Critical_Damage;

            Chargespeed = player.speed + count;                                         // 돌진모드의 속도 저장
            player.speed += count;                                                      // count만큼 속도 상승 [ 돌진모드 ] 
            Lance();

            yield return new WaitForSeconds(speed);                                     // speed 만큼 돌진모드 유지

            lancecharge = false;
            damage = memorydamage;
            player.speed = GameManager.instance.player.memoryspeed;                     // 그 후, 기억해둔 속도로 복구 [ 돌진종료 ]
            Lance();
        }
    }

    // 랜스 무기 변경 로직
    void LanceShieldMod()
    {
        Transform childTransform = transform.GetChild(0);                               // 자식오브젝트를 가져옴
        BoxCollider2D childCollider = childTransform.GetComponent<BoxCollider2D>();     // 자식오브젝트의, BoxCollider2D를 가져옴

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();                      // 자식오브젝트의 스프라이트를 초기화

        if (damage == Critical_Damage)                                                  // 데미지 == 크리티컬 데미지라는 뜻은 돌진모드 라는 뜻
        {
            //spriteRenderer.sprite = data.weaponimage[0];

            childCollider.offset = new Vector2(0f, 0.47f);                              // 포지션과, 사이즈를 창에 맞게 변경함
            childCollider.size = new Vector2(0.5f, 1.8f);
            childTransform.transform.localPosition = new Vector3(0f, 0.8f, 0f);

            LanceRotate();
        }
        else if (lancecharge == false)                                                  // 돌진모드가 끝나면
        {
            spriteRenderer.sprite = data.weaponimage[1];                                // 이미지를 방패로 바꾸고

            transform.rotation = Quaternion.Euler(0f, 0f, 0f);                          // 방패에 맞게 사이즈, 포지션, 피격범위 등을 수정함

            childCollider.offset = new Vector2(0f, 0f);
            childCollider.size = new Vector2(1.0f, 1.0f);

            childTransform.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
}