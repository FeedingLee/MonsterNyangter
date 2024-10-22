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

        switch (data.itemType)
        {
            case ItemData.ItemType.DualBlades:
                textDesc.text = string.Format(data.itemDesc,
                    data.damages[level] * 100,                    // 데미지 상승량         
                    data.W_Speeds[level] * 100,                   // 무기 공격속도 상승량
                    data.counts[level]);                          // 무기 회전 갯수
                break;
            case ItemData.ItemType.HuntingBow:
                textDesc.text = string.Format(data.itemDesc, 
                    data.damages[level] * 100,                    // 데미지 상승량         
                    data.W_Rates[level] * 100);                   // 무기 연사속도 상승량
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc); 
                break;
        }
    }

    public void OnClick()
    {
        switch(data.itemType)
        {
            case ItemData.ItemType.DualBlades:
            case ItemData.ItemType.HuntingBow:
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

                    // 레벨업 스펙 상승량 설명 로직
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
