using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item", menuName ="Scriptble Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType 
    {  
        DualBlades, HuntingBow, SniperHBG , GreatSword, Lance,          // 메인무기
        Rate_Up, Speed_Up, Heal, Damage_Up, Stamina_Up, Weapon_Upgrade, Magnet
    }
    
    [Header("Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;

    [Header("Weapon Base Data")]
    public float baseDamage;
    public float baseSpeed;   // 회전 속도
    public float baseRate;    // 연사 속도
    public int baseCount;

    [Header("Weapon LevelUp Data")]   
    public float[] damages;
    public float[] W_Speeds;
    public float[] W_Rates;
    public int[] counts;

    [Header("Weapon Data [/MHS Wepaon Data]")]
    public GameObject projectile;

    [Header("Weapon Another Image")]
    public Sprite[] weaponimage;
}
