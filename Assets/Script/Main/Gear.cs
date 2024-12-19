using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float power; // 스탯이 레벨업 시 가질 능력치
    public static float rate_stat; // 연사속도 스텟
    public static float speed_stat; // 이동속도 스텟

    public void Init(ItemData data)
    {
        // 기본세팅
        if (data.itemId == 5)
        {
            name = "Rate_Up [" + data.itemId + "]";
        }
        if (data.itemId == 6)
        {
            name = "Speed_Up [" + data.itemId + "]";
        }
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        // 값 세팅
        type = data.itemType;
        power = data.damages[0];

        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.power = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Rate_Up:
                rate_stat = power; // 연사속도 스텟을, 연사의 데미지 만큼 적용
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                speed_stat = power; // 이동속도 스텟을, 이동속도의 데미지 만큼 적용
                SpeedUp();
                break;
        }
    }

    // 연사속도 스텟 증가 로직
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:     // 쌍검은 Rate의 영향이 없음
                    break;
                default:    // 0번 이외 = 원거리무기
                    weapon.rate = weapon.rate * (1.0f - power); // 현재 가진 무기들 스펙 업
                    break;
            }
        }
    }

    // 이동속도 스텟 증가 로직
    void SpeedUp()
    {
        float speed = 4 * Character.Speed; // 숫자는 플레이어의 기본속도를 의미함
        GameManager.instance.player.speed = speed + (speed * power);
    }
}
