using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public float speed;     // 무기의 회전 속도
    public float rate;      // 무기의 연사 속도
    public int count;       // 회전하는 무기 갯수

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
            case 1 :
                timer += Time.deltaTime;
                
                if (timer > rate)
                {
                    timer = 0f;
                    BowShoot();
                }
                break;
            // 저격용탄 헤비보우건의 공격 방식
            case 2 :
                timer += Time.deltaTime;

                if (timer > rate)
                {
                    timer = 0f;
                    Sniper();
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
            Batch();
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
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

        // 만약 Case 0 무기라면, 바로 Batch()를 호출하여 자식 오브젝트 생성
        if (id == 0)
        {
            Batch();  // 무기 초기화 시점에서 Batch 호출
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 쌍검 공격 로직
    void Batch()
    {
        for (int index=0; index < count; index++)
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // -100 은 관통력 무한.
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

    // 조이스틱의 최종 방향을 받아오고, 총알을 일직선으로 발사하는 로직
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
}
