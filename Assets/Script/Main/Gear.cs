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
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    // 장갑의 연사력 증가 로직 [ 근데 이거 클릭하면 초기화 됨 그래서 안쓰거나 바꾸거나 해야함 ]
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:     // 0번 = 쌍검
                    //float speed = weapon.speed + (150 - (150 * Character.WeaponSpeed)) + (150 * rate);
                    //weapon.speed = speed;
                    break;
                case 3:     // 3번 = 대검
                    //speed = 1000 * Character.WeaponRate;
                    //weapon.rate = rate * 0.5f * (1f - rate);
                    break;
                default:    // 0번 이외 = 원거리무기
                    //speed = 0.9f * Character.WeaponRate;
                    //weapon.rate = weapon.rate * (0.9f - rate);
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
