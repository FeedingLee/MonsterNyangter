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
            name = "Gear " + data.itemId;
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
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    // 장갑의 연사력 증가 로직 
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:     // 0번 = 쌍검
                case 3:     // 3번 = 대검 
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.speed = 150 + (150 * rate);
                    break;
                default:    // 0번 이외 = 원거리무기
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = speed * 0.5f * (1f - rate);
                    break;
            }
        }
    }

    // 신발의 이동속도 증가 로직
    void SpeedUp()
    {
        float speed = 3 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
