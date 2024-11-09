using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public float speed;      // 무기의 회전 속도
    public float rate;       // 무기의 연사 속도
    public int count;        // 회전하는 무기 갯수
    bool charm = false;      // 대검의 회전중을 확인하는 변수

    float timer;
    Player player;
    ItemData data;
    Item item;
    JoystickController joystickController;

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
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            // 활의 공격 방식
            case 1:
                timer += Time.deltaTime;

                if (timer > rate)
                {
                    timer = 0f;
                    BowShoot();
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
                break;
            // 대검의 공격 방식 - 회전 중이 아닐 때만 호출
            case 3:
                if (!charm)       // 회전 중이 아닐 때만 실행
                {
                    charm = true; // 회전 중으로 설정
                    StartCoroutine(GreatSwordRotate());
                }
                break;
        }
    }

    public void LevelUp(float damage, float speed, float rate, int count)
    {
        this.damage = damage * Character.Damage;
        this.speed = speed * Character.WeaponSpeed;
        this.rate = rate * Character.WeaponRate;
        this.count = count;

        if (id == 0)
        {
            DualBlades();
        }
        if (id == 3)
        {
            GreatSword();
        }
    }

    public void Init(ItemData data)
    {
        // Basic Set
        this.data = data;
        name = "Weapon" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // 무기 능력치 셋팅
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        speed = data.baseSpeed * Character.WeaponSpeed;
        rate = data.baseRate * Character.WeaponRate;
        count = (int)(data.baseCount + Character.Count);

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
            GreatSword();
        }
    }

    // 쌍검 공격 로직
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

    // 대검 공격 함수
    void GreatSword()
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

            Vector3 rotVec = Vector3.forward * 360;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 3.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }

    // 적을 추적하여 자동으로 총알을 발사하는 로직
    void BowShoot()
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

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }

    // 저격용탄 헤비보우건 공격 함수
    // 조이스틱의 최종 방향을 받아오고, 총알을 일직선으로 발사함
    void Sniper()
    {
        Vector2 dir = joystickController.GetInputVector();  // Joystick 방향 벡터 받아옴                                              
        if (dir == Vector2.zero)                            // 정지상태에서는 무작위로 발사
        {
            float randomAngle = Random.Range(0f, 360f);
            dir = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)).normalized;
        }

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Sniper);  // 발사 소리
    }

    // 대검 회전 코루틴
    IEnumerator GreatSwordRotate()
    {
        int rotationCount = 0;                                                          // 회전 횟수를 추적하기 위한 변수

        while (rotationCount < count)                                                   // count 횟수만큼 회전을 반복
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.GreatSword);                 // 무기 사운드
            float rotatedAmount = 0f;                                                   // 회전 누적값을 초기화         

            while (rotatedAmount < (360f * count))                                      // count 수만큼(바퀴) 360도를 회전
            {
                float rotationStep = speed * Time.deltaTime;                            // 매 프레임마다 회전할 양 계산
                transform.Rotate(Vector3.back * rotationStep);                          // 실제 회전 수행
                rotatedAmount += rotationStep;                                          // 누적 회전량 증가
                yield return null;                                                      // 한 프레임 대기
            }
            Vector2 dir = joystickController.GetInputVector();                          // Joystick 방향 벡터 받아옴
            transform.localRotation = Quaternion.FromToRotation(Vector3.up, dir);       // 마지막 조이스틱 방향으로, 대검을 위치함

            rotationCount++;                                                            // 한 번의 360도 회전이 끝날 때마다 회전 횟수 증

            yield return new WaitForSeconds(rate);                                      // 한 번 회전이 끝난 후 rate만큼 대기
        }
        charm = false;                                                                  // 참모아베기 정지상태로 변경
    }
}
