using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;
    public static int Exp_Bonus;
    public static int Super_Magnet;

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
                    textDesc.text = string.Format("회전하며 \n마구 벤다냥!");
                    break;
                case ItemData.ItemType.HuntingBow:
                    textDesc.text = string.Format("백발백중!\n반드시\n맞춘다냥!");
                    break;
                case ItemData.ItemType.SniperHBG:
                    textDesc.text = string.Format("내 앞에서\n사라지라냥!");
                    break;
                case ItemData.ItemType.GreatSword:
                    textDesc.text = string.Format("참 모아..라고\n알고있냥?");
                    break;
                case ItemData.ItemType.Lance:
                    textDesc.text = string.Format("이쑤시개\n아니다냥.");
                    break;
                case ItemData.ItemType.Rate_Up:
                case ItemData.ItemType.Speed_Up:
                case ItemData.ItemType.Damage_Up:
                case ItemData.ItemType.Stamina_Up:
                case ItemData.ItemType.Weapon_Upgrade:
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
                case ItemData.ItemType.GreatSword :
                case ItemData.ItemType.Lance :
                    textDesc.text = string.Format(data.itemDesc,
                        data.baseDamage,                                                        // 기본 데미지 [0]
                        data.baseDamage + (data.baseDamage * data.damages[level]),              // 최종 데미지 [1]
                        data.baseSpeed + (data.W_Speeds[level] * 100),                          // 최종 회전 속도 [2]
                        data.baseCount + (data.counts[level]),                                  // 최종 회전체 갯수 [3]
                        data.baseRate + (data.baseRate * data.W_Rates[level]),                  // 최종 쿨타임 [4]
                        data.baseSpeed + (data.baseSpeed * data.W_Speeds[level]),               // 돌진유지 시간 [5]
                        (data.baseDamage + (data.baseDamage * data.damages[level])) * 1.5f);    // 대검 데미지 [6]
                    break;
                case ItemData.ItemType.HuntingBow:
                case ItemData.ItemType.SniperHBG:
                    textDesc.text = string.Format(data.itemDesc,
                        data.baseDamage,                                            // 기본 데미지 [0]
                        data.baseDamage + (data.baseDamage * data.damages[level]),  // 최종 데미지 [1]
                        data.baseRate + (data.baseRate * data.W_Rates[level]),      // 최종 연사속도 [2]
                        data.baseCount + (data.counts[level]));                     // 최종 관통력 [3]
                    break;
                case ItemData.ItemType.Rate_Up:
                case ItemData.ItemType.Speed_Up:
                case ItemData.ItemType.Damage_Up:
                case ItemData.ItemType.Stamina_Up:
                case ItemData.ItemType.Weapon_Upgrade:
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
            case ItemData.ItemType.GreatSword:
            case ItemData.ItemType.Lance:
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
            case ItemData.ItemType.Rate_Up:
            case ItemData.ItemType.Speed_Up:
            case ItemData.ItemType.Damage_Up:
            case ItemData.ItemType.Stamina_Up:
            case ItemData.ItemType.Weapon_Upgrade:
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
            case ItemData.ItemType.Magnet:
                GameManager.instance.player.scanner.expScanRange ++;
                break;
            case ItemData.ItemType.Exp_Coupon:
                Exp_Bonus = 1;   
                GameManager.expbonuscheck = 0;
                break;
            case ItemData.ItemType.Super_Magnet:
                Super_Magnet = 1;
                GameManager.supermagnetcheck = 0;
                break;
        }

        if (level == data.damages.Length) 
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
