using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName;
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);
        if (level == 0)
        {
            switch (data.itemType)                                     // 최초 획득에 따른 무기 설명
            {
                case ItemData.ItemType.DualBlades:
                    textDesc.text = string.Format("회전하며 마구 벤다냥!");
                    break;
                case ItemData.ItemType.HuntingBow:
                    textDesc.text = string.Format("백발백중!\n반드시 맞춘다냥!");
                    break;
                case ItemData.ItemType.SniperHBG:
                    textDesc.text = string.Format("엄청강한 한발!\n로망있지않냥?");
                    break;
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                    textDesc.text = string.Format(data.itemDesc,
                        data.damages[level] * 100);
                    break;
                default:
                    textDesc.text = string.Format(data.itemDesc);
                    break;
            }
        }
        else
        {
            switch (data.itemType)                                     // 레벨업에 따른 무기 설명
            {
                case ItemData.ItemType.DualBlades:
                    textDesc.text = string.Format(data.itemDesc,
                        data.baseDamage,                               // 기본 데미지 [0]
                        data.baseSpeed,                                // 기본 회전속도 [1]
                        data.baseCount,                                // 기본 회전체 갯수 [2]
                        data.damages[level] * 10,                      // 레벨당 데미지 상승량 [3]
                        data.W_Speeds[level] * 100,                    // 레벨당 회전속도 상승량 [4]
                        data.counts[level],                            // 레벨당 회전체 갯수 상승량 [5]
                        data.baseDamage + (data.damages[level] * 10),  // 최종 데미지 [6]
                        data.baseSpeed + (data.W_Speeds[level] * 100), // 최종 회전 속도 [7]
                        data.baseCount + (data.counts[level]));        // 최종 회전체 갯수 [8]
                    break;
                case ItemData.ItemType.HuntingBow:
                case ItemData.ItemType.SniperHBG:
                    textDesc.text = string.Format(data.itemDesc,
                        data.baseDamage,                                // 기본 데미지 [0]
                        data.baseRate,                                  // 기본 연사속도 [1]
                        data.baseCount,                                 // 기본 회전체 갯수 [2]
                        data.baseDamage * data.damages[level],                      // 레벨당 데미지 상승량 [3]
                        data.W_Rates[level] * -100,                     // 레벨당 연사속도 상승량 [4]
                        data.counts[level],                             // 레벨당 관통력 상승량 [5]
                        data.baseDamage + (data.baseDamage * data.damages[level]),   // 최종 데미지 [6]
                        data.baseRate + (data.baseRate * data.W_Rates[level]),  // 최종 연사속도 [7]
                        data.baseCount + (data.counts[level]));         // 최종 관통력 [8]
                    break;
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                    textDesc.text = string.Format(data.itemDesc,
                        data.damages[level] * 100);
                    break;
                default:
                    textDesc.text = string.Format(data.itemDesc);
                    break;
            }
        }
    }

    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.DualBlades:
            case ItemData.ItemType.HuntingBow:
            case ItemData.ItemType.SniperHBG:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);  
                }
                else
                {
                    float nextDamage = data.baseDamage;     // 무기 데미지
                    float nextSpeed = data.baseSpeed;       // 무기 회전 속도
                    float nextRate = data.baseRate;         // 무기 연사 속도
                    int nextCount = data.baseCount;                      

                    nextDamage += data.baseDamage * data.damages[level];
                    nextSpeed += data.baseSpeed * data.W_Speeds[level];
                    nextRate += data.baseRate * data.W_Rates[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextSpeed, nextRate, nextCount);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }

                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        if (level == data.damages.Length) 
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
