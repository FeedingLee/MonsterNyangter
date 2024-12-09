using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;
    ItemData data;

    public void Init(ItemData data)
        {
            // Basic Set
            name = "Gear" + data.itemId;
            transform.parent = GameManager.instance.player.transform;
            transform.localPosition = Vector3.zero;

            // Property Set
            type = data.itemType;
            rate = data.damages[0];
            ApplyGear();
        }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Rate_Up:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
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
                    weapon.rate = weapon.rate * (1.0f - rate);
                    break;
            }
        }
    }

    // 이동속도 스텟 증가 로직
    void SpeedUp()
    {
        float speed = 4 * Character.Speed; // 숫자는 플레이어의 기본속도를 의미함
        GameManager.instance.player.speed = speed + (speed * rate);
    }
}
