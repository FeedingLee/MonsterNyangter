using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Text textLevel;                                  // 텍스트를 받아올 변수
    public Image WeaponImg;                                 // 무기 이미지를 변경할 Image
    public Sprite ChangeImg;                                // 바꿀 이미지 저장

    public Object weapon_data;                              // 무기의 상태를 가지고 있는, Weapon 오브젝트 받아오기
    public int level;                                       // 레벨을 저장

    private void Start()                                            
    {
        // 첫 번째 자식 오브젝트의 Image 컴포넌트를 가져오기
        WeaponImg = transform.GetChild(0).GetComponent<Image>();

        // 모든 자식 Text 컴포넌트를 배열로 가져오기
        Text[] text = GetComponentsInChildren<Text>();
        textLevel = text[0];                                // 첫 번째 Text 컴포넌트를 Level 텍스트로 할당
    }

    private void Update()
    {
        // 데이터 오브젝트의 레벨이 현재 레벨과 다르면 업데이트
        if (level != weapon_data.GetComponent<Item>().level)
        {
            level = weapon_data.GetComponent<Item>().level; // 현재 Level 업데이트
            textLevel.text = "Lv." + level;                 // 레벨 텍스트 업데이트
        }

        // 만렙일 시 첫 번째 자식의 Image를 변경
        if (level == 5)
        {
            WeaponImg.sprite = ChangeImg;                  // 만렙일 시 무기 아이콘 변경
        }
    }
}