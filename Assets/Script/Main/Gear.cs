using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public Weapon Weapon;

    public float power;               // 능력치 레벨업 시 가질 능력치
    public static float rate_stat;    // 연사속도 능력치
    public static float damage_stat;  // 데미지 능력치

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
        if (data.itemId == 8)
        {
            name = "Damage_Up [" + data.itemId + "]";
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
                rate_stat = power; // 연사속도 능력치를, 연사의 데미지 만큼 적용
                RateUp();
                break;
            case ItemData.ItemType.Speed_Up:
                SpeedUp();
                break;
            case ItemData.ItemType.Damage_Up:
                damage_stat = power; // 이동속도 능력치를, 이동속도의 데미지 만큼 적용
                DamageUp();
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
        float speed = 3 * Character.Speed; // 숫자는 플레이어의 기본속도를 의미함
        GameManager.instance.player.speed = speed + (speed * power);
        GameManager.instance.player.memoryspeed = speed + (speed * power);  // 테스트
    }

    // 공격력 스텟 증가 로직
    void DamageUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                default:    
                    weapon.damage = weapon.damage * (1.0f + power);                     // 데미지 상승
                    weapon.Critical_Damage = weapon.Critical_Damage * (1.0f + power);   // 크리티컬 데미지 상승
                    weapon.memorydamage = weapon.memorydamage * (1.0f + power);         // memorydamage 값 상승
                    break;
            }          
        }
    }
}
